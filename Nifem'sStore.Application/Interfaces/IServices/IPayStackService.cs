using NifemsStores.Application.Common.RequestModel.PaymentDTO;
using NifemsStores.Application.Common.Response;
using NifemsStores.Application.DTOs;

namespace NifemsStores.Application.Interfaces.IServices
{
    public interface IPayStackService
    {
        Task<BaseResponse<InitializePaymentResponseDto>> InitializePaymentAsync(InitializePaymentRequestDto requestDto);
        Task<BaseResponse<VerifyPaymentRequestDto>> VerifyPaymentAsync(string reference);
        Task ProcessWebhookAsync(string json);
        bool ValidateWebhookSignature(string json, string signature);
        Task<string?> EnsureSplitAsync();
    }
}
