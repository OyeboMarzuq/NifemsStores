using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NifemsStores.Application.Interfaces.IServices;

namespace NifemsStores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class PurchaseReportController : ControllerBase
    {
        private readonly IPurchaseReportService _purchaseReportService;

        public PurchaseReportController(IPurchaseReportService purchaseReportService)
        {
            _purchaseReportService = purchaseReportService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPurchaseReports()
        {
            var response = await _purchaseReportService.GetAllPurchaseReports();
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetPurchaseReportById(Guid reportId)
        {
            var response = await _purchaseReportService.GetPurchaseReportById(reportId);
            return StatusCode(response.StatusCode ?? 200, response);
        }
    }
}
