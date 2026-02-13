using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NifemsStore.Application.DTOs.RecieptDTO;
using NifemsStore.Application.Interfaces.IServices;
using System.Security.Claims;

namespace NifemsStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private string GetUserName()
        {
            return User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
        }

        private bool IsAdmin()
        {
            return User.IsInRole("ADMIN");
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReceipt([FromBody] CreateReceiptDto dto)
        {
            var response = await _receiptService.CreateReceipt(dto, GetUserId(), GetUserName());
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpGet("my-receipts")]
        public async Task<IActionResult> GetMyReceipts()
        {
            var response = await _receiptService.GetMyReceipts(GetUserId());
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpGet("{receiptId}")]
        public async Task<IActionResult> GetReceiptById(Guid receiptId)
        {
            var response = await _receiptService.GetReceiptById(receiptId, GetUserId(), IsAdmin());
            return StatusCode(response.StatusCode ?? 500, response);
        }
    }
}
