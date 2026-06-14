using Microsoft.AspNetCore.Mvc;
using NifemsStore.Application.DTOs.ProductDTO;
using NifemsStore.Application.Interfaces.IServices;

namespace NifemsStores.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpPost("generate-from-sale")]
        public async Task<IActionResult> GenerateFromSale([FromBody] ProductSaleDto sale)
        {
            if (sale == null)
                return BadRequest("Invalid sale data.");

            var (receipt, pdfBytes) =
                await _receiptService.GenerateFromProductSaleAsync(sale);

            return File(pdfBytes, "application/pdf",
                $"{receipt.ReceiptNumber}.pdf");
        }
    }
}