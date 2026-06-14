using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NifemsStores.Application.Common.RequestModel.PaymentDTO;
using NifemsStores.Application.Common.Response;
using NifemsStores.Application.DTOs;
using NifemsStores.Application.Interfaces.IServices;
using NifemsStores.Domain.Entities;
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
        private readonly IConfiguration _configuration;
        private readonly ILogger<PayStackService> _logger;
        private readonly string _secretKey;

        public PayStackService(
            IHttpClientFactory httpClientFactory,
            ApplicationDbContext dbcontext,
            IConfiguration configuration,
            ILogger<PayStackService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _dbcontext = dbcontext;
            _configuration = configuration;
            _logger = logger;
            _secretKey = configuration["Paystack:SecretKey"]!;
        }

        // ─────────────────────────────────────────────────────────────
        // SPLIT: Ensure a Paystack split exists; create it if not.
        // Call this once on startup (see Program.cs) or lazily here.
        // ─────────────────────────────────────────────────────────────
        public async Task<string?> EnsureSplitAsync()
        {
            // Return cached split_code from DB if already created
            var existing = await _dbcontext.PaystackSplits.FirstOrDefaultAsync();
            if (existing != null)
            {
                _logger.LogInformation("Using cached Paystack split: {SplitCode}", existing.SplitCode);
                return existing.SplitCode;
            }

            // Build the subaccounts list
            // Owner account: Moniepoint 7041061536 — 90%
            // 10% account: OPay — fill in account number below when ready
            var ownerAccountNumber   = _configuration["Paystack:OwnerAccountNumber"];   // 7041061536
            var ownerBankCode        = _configuration["Paystack:OwnerBankCode"];         // Moniepoint: 50515
            var secondAccountNumber  = _configuration["Paystack:SecondAccountNumber"];   // OPay account (fill later)
            var secondBankCode       = _configuration["Paystack:SecondBankCode"];        // OPay: 999992

            // Resolve subaccount codes from Paystack (account number → subaccount_code)
            var ownerSubaccountCode  = await ResolveOrCreateSubaccountAsync(
                ownerAccountNumber!, ownerBankCode!, "NifemsCollection Owner", 90);

            string? splitCode;

            if (!string.IsNullOrWhiteSpace(secondAccountNumber))
            {
                var secondSubaccountCode = await ResolveOrCreateSubaccountAsync(
                    secondAccountNumber, secondBankCode!, "NifemsCollection Partner", 10);

                splitCode = await CreateSplitAsync("NifemsCollection Split", new[]
                {
                    (ownerSubaccountCode, 9000),   // 90% in basis points (Paystack uses integer %)
                    (secondSubaccountCode, 1000)    // 10%
                });
            }
            else
            {
                // Second account not configured yet — single subaccount, full 100% to owner
                _logger.LogWarning("Second (OPay) account not configured. Split will send 100% to owner until filled in.");
                splitCode = await CreateSplitAsync("NifemsCollection Split", new[]
                {
                    (ownerSubaccountCode, 10000)
                });
            }

            if (splitCode != null)
            {
                _dbcontext.PaystackSplits.Add(new PaystackSplit
                {
                    SplitCode = splitCode,
                    SplitName = "NifemsCollection Split"
                });
                await _dbcontext.SaveChangesAsync();
                _logger.LogInformation("Paystack split created and cached: {SplitCode}", splitCode);
            }

            return splitCode;
        }

        // ─────────────────────────────────────────────────────────────
        // INITIALIZE PAYMENT — attaches split_code to every transaction
        // ─────────────────────────────────────────────────────────────
        public async Task<BaseResponse<InitializePaymentResponseDto>> InitializePaymentAsync(
            InitializePaymentRequestDto requestDto)
        {
            try
            {
                if (requestDto.Amount <= 0)
                    return BaseResponse<InitializePaymentResponseDto>.Failure("Invalid payment amount", statusCode: 400);

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

                // Fetch the split code (from DB cache or create fresh)
                var splitCode = await EnsureSplitAsync();

                var callbackUrl = _configuration["Paystack:CallbackUrl"] ?? "https://yourdomain.com/payment/callback";

                // Build payload — attach split_code if available
                object payload = splitCode != null
                    ? new
                    {
                        amount       = (long)(requestDto.Amount * 100),
                        email        = requestDto.Email.Trim(),
                        reference,
                        callback_url = callbackUrl,
                        split_code   = splitCode
                    }
                    : new
                    {
                        amount       = (long)(requestDto.Amount * 100),
                        email        = requestDto.Email.Trim(),
                        reference,
                        callback_url = callbackUrl
                    };

                var client = _httpClientFactory.CreateClient();
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.paystack.co/transaction/initialize");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);
                requestMessage.Content = new StringContent(
                    JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                var response = await client.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();
                var paystackResponse = JsonConvert.DeserializeObject<PaystackResponseDto<InitializePaymentResponseDto>>(responseContent);

                if (paystackResponse?.Status == true)
                {
                    paymentRequest.Status = "Initialized";
                    await _dbcontext.SaveChangesAsync();

                    return BaseResponse<InitializePaymentResponseDto>.Success(
                        paystackResponse.Data, "Payment initialized successfully", 200);
                }

                _logger.LogError("Paystack initialization failed: {Response}", responseContent);
                return BaseResponse<InitializePaymentResponseDto>.Failure("Payment initialization failed", statusCode: 502);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing Paystack payment");
                return BaseResponse<InitializePaymentResponseDto>.Failure("Something went wrong", statusCode: 500);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // WEBHOOK
        // ─────────────────────────────────────────────────────────────
        public async Task ProcessWebhookAsync(string json)
        {
            var webhookEvent = JsonConvert.DeserializeObject<PaystackWebhookDto>(json);
            if (webhookEvent == null || webhookEvent.Event != "charge.success")
                return;

            var reference = webhookEvent.Data.Reference;
            var verification = await VerifyPaymentAsync(reference);
            if (!verification.Success)
                return;

            var transaction = await _dbcontext.PaymentRequests
                .FirstOrDefaultAsync(x => x.TransactionReference == reference);

            if (transaction == null || transaction.Status == "Paid")
                return;

            if (verification.Data?.Data?.Amount != transaction.Amount * 100)
            {
                _logger.LogWarning("Amount mismatch on webhook for reference {Reference}", reference);
                return;
            }

            transaction.Status = "Paid";
            transaction.DatePaid = DateTime.UtcNow;
            await _dbcontext.SaveChangesAsync();

            _logger.LogInformation("Payment confirmed via webhook: {Reference}", reference);
        }

        public bool ValidateWebhookSignature(string json, string signature)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(json));
            var computed = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return computed == signature;
        }

        // ─────────────────────────────────────────────────────────────
        // VERIFY PAYMENT
        // ─────────────────────────────────────────────────────────────
        public async Task<BaseResponse<VerifyPaymentRequestDto>> VerifyPaymentAsync(string reference)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"https://api.paystack.co/transaction/verify/{reference}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);

                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                var verifyResponse = JsonConvert.DeserializeObject<VerifyPaymentRequestDto>(responseContent);

                return new BaseResponse<VerifyPaymentRequestDto>
                {
                    Success = verifyResponse?.Status ?? false,
                    Message = verifyResponse?.Message,
                    Data = verifyResponse
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying Paystack payment");
                return BaseResponse<VerifyPaymentRequestDto>.Failure("Something went wrong", statusCode: 500);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // PRIVATE HELPERS
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a Paystack subaccount for a bank account and returns the subaccount_code.
        /// If the subaccount already exists on Paystack, you'd get an error — in production
        /// you can store subaccount codes in config after first creation.
        /// </summary>
        private async Task<string> ResolveOrCreateSubaccountAsync(
            string accountNumber, string bankCode, string businessName, int percentageCharge)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.paystack.co/subaccount");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);

            var payload = new
            {
                business_name      = businessName,
                settlement_bank    = bankCode,
                account_number     = accountNumber,
                percentage_charge  = percentageCharge
            };

            request.Content = new StringContent(
                JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(json);

            string subaccountCode = result?.data?.subaccount_code ?? throw new Exception(
                $"Failed to create subaccount for {accountNumber}: {json}");

            _logger.LogInformation("Subaccount created: {Code} for {Account}", subaccountCode, accountNumber);
            return subaccountCode;
        }

        /// <summary>
        /// Creates a Paystack Split and returns the split_code.
        /// Shares are in percentage basis points (10000 = 100%).
        /// </summary>
        private async Task<string> CreateSplitAsync(string name, IEnumerable<(string subaccountCode, int share)> subaccounts)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.paystack.co/split");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _secretKey);

            var payload = new
            {
                name,
                type       = "percentage",
                currency   = "NGN",
                subaccounts = subaccounts.Select(s => new
                {
                    subaccount = s.subaccountCode,
                    share      = s.share
                })
            };

            request.Content = new StringContent(
                JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(json);

            string splitCode = result?.data?.split_code ?? throw new Exception(
                $"Failed to create split: {json}");

            return splitCode;
        }
    }
}
