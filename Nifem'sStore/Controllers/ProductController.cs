using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NifemsStores.Application.DTOs;
using NifemsStores.Application.DTOs.ProductDTO;
using NifemsStores.Application.Interfaces.IServices;
using System.Security.Claims;

namespace NifemsStores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        private string GetUserName() => User.FindFirstValue(ClaimTypes.Name) ?? "Admin";

        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestDto dto)
        {
            var response = await _productService.CreateProduct(dto, GetUserName());
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto dto)
        {
            var response = await _productService.UpdateProduct(dto, GetUserName());
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpDelete("delete/{productId}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var response = await _productService.DeleteProduct(productId, GetUserName());
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [AllowAnonymous]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductFilterRequestDto filter)
        {
            var response = await _productService.GetAllProducts(filter);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [AllowAnonymous]
        [HttpGet("get/{productId}")]
        public async Task<IActionResult> GetProductById(Guid productId)
        {
            var response = await _productService.GetProductById(productId);
            return StatusCode(response.StatusCode ?? 200, response);
        }
    }
}
