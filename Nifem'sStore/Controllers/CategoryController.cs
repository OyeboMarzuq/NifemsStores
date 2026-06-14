using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NifemsStores.Application.DTOs.CategoryDTO;
using NifemsStores.Application.Interfaces.IServices;
using System.Security.Claims;

namespace NifemsStores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        private string GetUserName() => User.FindFirstValue(ClaimTypes.Name) ?? "Admin";

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto dto)
        {
            var response = await _categoryService.CreateCategory(dto, GetUserName());
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [AllowAnonymous]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _categoryService.GetAllCategories();
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [AllowAnonymous]
        [HttpGet("get/{categoryId}")]
        public async Task<IActionResult> GetCategoryById(Guid categoryId)
        {
            var response = await _categoryService.GetCategoryById(categoryId);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("update/{categoryId}")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] UpdateCategoryDto dto)
        {
            var response = await _categoryService.UpdateCategory(categoryId, dto, GetUserName());
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("delete/{categoryId}")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            var response = await _categoryService.DeleteCategory(categoryId, GetUserName());
            return StatusCode(response.StatusCode ?? 200, response);
        }
    }
}
