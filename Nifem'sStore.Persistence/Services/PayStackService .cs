using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NifemsStores.Application.Common.RequestModel.PaymentDTO;
using NifemsStores.Application.Common.Response;
using NifemsStores.Application.DTOs;
using NifemsStores.Application.Interfaces.IServices;
using NifemsStores.Domain.Entities;
using NifemsStores.Application.Common.Response;
using NifemsStores.Persistence.Context;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace NifemsStores.Persistence.Services
{
    public class PayStackService : IPayStackService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _dbcontext;
        private readonly string _secretKey;

        public PayStackService(IHttpClientFactory httpClientFactory, ApplicationDbContext dbcontext, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _dbcontext = dbcontext;
            _secretKey = configuration["Paystack:SecretKey"];
        }
        public async Task<BaseResponse<InitializePaymentResponseDto>> InitializePaymentAsync(
    InitializePaymentRequestDto requestDto)
        {
            try
            {
                if (requestDto.Amount <= 0)
                    throw new Exception("Invalid payment amount");

                var reference = string.IsNullOrEmpty(requestDto.Reference)
                    ? Guid.NewGuid().ToString("N")
                    : requestDto.Reference;

                var paymentRequest = new PaymentRequest
                {
                    Email = requestDto.Email,
                    Amount = requestDto.Amount,
                    Status = "Pending",
                    DateRequested = DateTime.UtcNow,
                    TransactionReference = reference
                };

                _dbcontext.PaymentRequests.Add(paymentRequest);
                await _dbcontext.SaveChangesAsync();

                var client = _httpClientFactory.CreateClient();

                var payload = new
                {
                    amount = requestDto.Amount * 100,
                    email = requestDto.Email.Trim(),
                    reference,
                    callback_url = "https://localhost:7009/PaymentCallback"
                };

                var requestMessage = new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://api.paystack.co/transaction/initialize");

                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", _secretKey);

                requestMessage.Content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.SendAsync(requestMessage);

                var responseContent = await response.Content.ReadAsStringAsync();

                var paystackResponse =
                    JsonConvert.DeserializeObject<PaystackResponseDto<InitializePaymentResponseDto>>(responseContent);

                if (paystackResponse != null && paystackResponse.Status)
                {
                    paymentRequest.Status = "Initialized";
                    await _dbcontext.SaveChangesAsync();

                    return new BaseResponse<InitializePaymentResponseDto>
                    {
                        Success = true,
                        Message = "Payment initialized successfully",
                        Data = paystackResponse.Data
                    };
                }

                return new BaseResponse<InitializePaymentResponseDto>
                {
                    Success = false,
                    Message = "Payment initialization failed"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<InitializePaymentResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task ProcessWebhookAsync(string json)
        {
            var webhookEvent =
                JsonConvert.DeserializeObject<PaystackWebhookDto>(json);

            if (webhookEvent == null)
                return;

            if (webhookEvent.Event != "charge.success")
                return;

            var reference = webhookEvent.Data.Reference;

            var verification = await VerifyPaymentAsync(reference);

            if (!verification.Success)
                return;

            var transaction = await _dbcontext.PaymentRequests
                .FirstOrDefaultAsync(x => x.TransactionReference == reference);

            if (transaction == null)
                return;

            if (transaction.Status == "Paid")
                return;

            if (verification.Data.Data.Amount != transaction.Amount * 100)
                return;

            transaction.Status = "Paid";
            transaction.DatePaid = DateTime.UtcNow;

            await _dbcontext.SaveChangesAsync();
        }

        public bool ValidateWebhookSignature(string json, string signature)
        {
            using var hmac = new HMACSHA512(
                Encoding.UTF8.GetBytes(_secretKey));

            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(json));

            var computedSignature =
                BitConverter.ToString(hash).Replace("-", "").ToLower();

            return computedSignature == signature;
        }

        public async Task<BaseResponse<VerifyPaymentRequestDto>> VerifyPaymentAsync(string reference)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"https://api.paystack.co/transaction/verify/{reference}");

                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _secretKey);

                var response = await client.SendAsync(request);

                var responseContent = await response.Content.ReadAsStringAsync();

                var verifyResponse =
                    JsonConvert.DeserializeObject<VerifyPaymentRequestDto>(responseContent);

                return new BaseResponse<VerifyPaymentRequestDto>
                {
                    Success = verifyResponse?.Status ?? false,
                    Message = verifyResponse?.Message,
                    Data = verifyResponse
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<VerifyPaymentRequestDto>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
