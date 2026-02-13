using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NifemsStore.Application.DTOs.ReportDTO;
using NifemsStore.Application.Interfaces.IServices;
using System.Security.Claims;

namespace NifemsStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private bool IsAdmin()
        {
            return User.IsInRole("ADMIN");
        }

        [HttpPost("create")]
        [Authorize(Roles = "VENDOR,ADMIN")]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportDto dto)
        {
            var response = await _reportService.CreateReport(dto, GetUserId());
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpGet("my-reports")]
        [Authorize(Roles = "VENDOR,ADMIN")]
        public async Task<IActionResult> GetMyReports()
        {
            var response = await _reportService.GetMyReports(GetUserId());
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpGet("{reportId}")]
        [Authorize(Roles = "VENDOR,ADMIN")]
        public async Task<IActionResult> GetReportById(Guid reportId)
        {
            var response = await _reportService.GetReportById(reportId, GetUserId(), IsAdmin());
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpDelete("delete/{reportId}")]
        [Authorize(Roles = "VENDOR,ADMIN")]
        public async Task<IActionResult> DeleteReport(Guid reportId)
        {
            var response = await _reportService.DeleteReport(reportId, GetUserId(), IsAdmin());
            return StatusCode(response.StatusCode ?? 500, response);
        }
    }
}
