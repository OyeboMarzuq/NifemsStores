using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NifemsStores.Application.DTOs.BrandDTO;
using NifemsStores.Application.Interfaces.IServices;
using NifemsStores.Application.Common.Response;

namespace NifemsStores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(BaseResponse<BrandDto>), 201)]
        [ProducesResponseType(typeof(BaseResponse<BrandDto>), 409)]
        [ProducesResponseType(typeof(BaseResponse<BrandDto>), 400)]
        public async Task<IActionResult> CreateBrand([FromBody] CreateBrandRequestDto request)
        {
            var response = await _brandService.CreateBrand(request);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(BaseResponse<BrandDto>), 200)]
        public async Task<IActionResult> GetAllBrands()
        {
            var response = await _brandService.GetAllBrands();
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("get/{id}")]
        [ProducesResponseType(typeof(BaseResponse<BrandDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<BrandDto>), 404)]
        public async Task<IActionResult> GetBrandById([FromRoute] Guid id)
        {
            var response = await _brandService.GetBrandById(id);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpPut("update")]
        [ProducesResponseType(typeof(BaseResponse<BrandDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<BrandDto>), 404)]
        public async Task<IActionResult> UpdateBrand([FromBody] UpdateBrandDto request)
        {
            var response = await _brandService.UpdateBrand(request);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        [ProducesResponseType(typeof(BaseResponse<bool>), 404)]
        public async Task<IActionResult> DeleteBrand([FromRoute] Guid id)
        {
            var response = await _brandService.DeleteBrand(id);
            return StatusCode(response.StatusCode ?? 200, response);
        }
    }
}
