using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NifemsStores.Application.DTOs.ReportDTO;
using NifemsStores.Application.Interfaces.IServices;

namespace NifemsStores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportDto dto)
        {
            var response = await _reportService.CreateReport(dto);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllReports()
        {
            var response = await _reportService.GetAllReports();
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetReportById(Guid reportId)
        {
            var response = await _reportService.GetReportById(reportId);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpDelete("delete/{reportId}")]
        public async Task<IActionResult> DeleteReport(Guid reportId)
        {
            var response = await _reportService.DeleteReport(reportId);
            return StatusCode(response.StatusCode ?? 200, response);
        }
    }
}
