using Microsoft.AspNetCore.Mvc;
using NifemsStores.Application.Common.RequestModel.PaymentDTO;
using NifemsStores.Application.Common.Response;
using NifemsStores.Application.Interfaces.IServices;

namespace NifemsStores.API.Controllers
{
    [Route("api/paystack")]
    [ApiController]
    public class PayStackController : ControllerBase
    {
        private readonly IPayStackService _payStackService;

        public PayStackController(IPayStackService payStackService)
        {
            _payStackService = payStackService;
        }

        // ===============================
        // Initialize Payment
        // ===============================
        [HttpPost("initialize")]
        public async Task<IActionResult> InitializePayment([FromBody] InitializePaymentRequestDto request)
        {
            var result = await _payStackService.InitializePaymentAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // ===============================
        // Verify Payment
        // ===============================
        [HttpGet("verify/{reference}")]
        public async Task<IActionResult> VerifyPayment(string reference)
        {
            var result = await _payStackService.VerifyPaymentAsync(reference);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // ===============================
        // Webhook Endpoint (Very Important)
        // ===============================
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            var signature = Request.Headers["x-paystack-signature"].FirstOrDefault();

            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync();

            // Validate webhook signature
            if (!_payStackService.ValidateWebhookSignature(json, signature))
                return Unauthorized();

            await _payStackService.ProcessWebhookAsync(json);

            return Ok();
        }
    }
}