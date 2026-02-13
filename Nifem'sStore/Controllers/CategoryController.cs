using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NifemsStore.Application.DTOs.CategoryDTO;
using NifemsStore.Application.Interfaces.IServices;
using NifemsStores.Application.DTOs;
using NifemsStores.Application.Interfaces.IServices;
using System.Security.Claims;

namespace NifemsStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Vendor")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        private Guid GetVendorId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private string GetUserEmail()
        {
            return User.FindFirstValue(ClaimTypes.Email)!;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto dto)
        {
            var vendorId = GetVendorId();
            var email = GetUserEmail();

            var response = await _categoryService.CreateCategory(dto, vendorId, email);
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var vendorId = GetVendorId();

            var response = await _categoryService.GetAllCategories(vendorId);
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpGet("get-by-id/{categoryId}")]
        public async Task<IActionResult> GetById(Guid categoryId)
        {
            var vendorId = GetVendorId();

            var response = await _categoryService.GetCategoryById(categoryId, vendorId);
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpPut("update/{categoryId}")]
        public async Task<IActionResult> Update(Guid categoryId, [FromBody] UpdateCategoryDto dto)
        {
            var vendorId = GetVendorId();
            var email = GetUserEmail();

            var response = await _categoryService.UpdateCategory(categoryId, dto, vendorId, email);
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpDelete("delete/{categoryId}")]
        public async Task<IActionResult> Delete(Guid categoryId)
        {
            var vendorId = GetVendorId();
            var email = GetUserEmail();

            var response = await _categoryService.DeleteCategory(categoryId, vendorId, email);
            return StatusCode(response.StatusCode ?? 500, response);
        }
    }
}
