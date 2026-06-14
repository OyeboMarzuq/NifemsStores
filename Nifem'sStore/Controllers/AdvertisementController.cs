using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NifemsStore.Application.DTOs.AdvertisementDTO;
using NifemsStore.Application.Interfaces.IServices;

namespace NifemsStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementController : ControllerBase
    {
        private readonly IAdvertisementService _advertisementService;

        public AdvertisementController(IAdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateAdvertisementDto dto)
        {
            var performedBy = User.Identity?.Name ?? "Unknown";

            var response = await _advertisementService.CreateAsync(dto, performedBy);
            return StatusCode(response.StatusCode ?? 500, response);
        }

        // ✅ Only Admin should update ads
        [Authorize(Roles = "ADMIN")]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateAdvertisementDto dto)
        {
            var performedBy = User.Identity?.Name ?? "Unknown";

            var response = await _advertisementService.UpdateAsync(dto, performedBy);
            return StatusCode(response.StatusCode ?? 500, response);
        }

        // ✅ Only Admin should delete ads
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var performedBy = User.Identity?.Name ?? "Unknown";

            var response = await _advertisementService.DeleteAsync(id, performedBy);
            return StatusCode(response.StatusCode ?? 500, response);
        }
    }
}