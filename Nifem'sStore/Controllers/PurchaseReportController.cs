using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NifemsStore.Application.Interfaces.IServices;
using System.Security.Claims;

namespace NifemsStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchaseReportController : ControllerBase
    {
        private readonly IPurchaseReportService _purchaseReportService;

        public PurchaseReportController(IPurchaseReportService purchaseReportService)
        {
            _purchaseReportService = purchaseReportService;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private bool IsAdmin()
        {
            return User.IsInRole("ADMIN");
        }

        [HttpGet("all")]
        [Authorize(Roles = "VENDOR,ADMIN")]
        public async Task<IActionResult> GetPurchaseReports()
        {
            var response = await _purchaseReportService.GetVendorPurchaseReports(GetUserId(), IsAdmin());
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpGet("{reportId}")]
        [Authorize(Roles = "VENDOR,ADMIN")]
        public async Task<IActionResult> GetPurchaseReportById(Guid reportId)
        {
            var response = await _purchaseReportService.GetPurchaseReportById(reportId, GetUserId(), IsAdmin());
            return StatusCode(response.StatusCode ?? 500, response);
        }
    }
}
