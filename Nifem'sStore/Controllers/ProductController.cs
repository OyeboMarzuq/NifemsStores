using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NifemsStore.Application.DTOs;
using NifemsStore.Application.DTOs.ProductDTO;
using NifemsStore.Application.Interfaces.IServices;
using System.Security.Claims;

namespace NifemsStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private string GetUserName()
        {
            return User.FindFirstValue(ClaimTypes.Name);
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        // CREATE
        [Authorize(Roles = "Vendor,Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequestDto dto)
        {
            var vendorId = GetUserId();
            var userName = GetUserName();

            var response = await _productService.CreateProduct(dto, vendorId, userName);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // UPDATE
        [Authorize(Roles = "Vendor,Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto dto)
        {
            var vendorId = GetUserId();
            var isAdmin = IsAdmin();

            var response = await _productService.UpdateProduct(dto, vendorId, isAdmin);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // DELETE
        [Authorize(Roles = "Vendor,Admin")]
        [HttpDelete("delete/{productId}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var vendorId = GetUserId();
            var isAdmin = IsAdmin();

            var response = await _productService.DeleteProduct(productId, vendorId, isAdmin);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // GET ALL WITH PAGINATION + FILTER
        [Authorize(Roles = "Vendor,Admin")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductFilterRequestDto filter)
        {
            var vendorId = GetUserId();
            var isAdmin = IsAdmin();

            var response = await _productService.GetAllProducts(filter, vendorId, isAdmin);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // GET BY ID
        [Authorize(Roles = "Vendor,Admin")]
        [HttpGet("get/{productId}")]
        public async Task<IActionResult> GetProductById(Guid productId)
        {
            var response = await _productService.GetProductById(productId);
            return StatusCode(response.StatusCode ?? 200, response);
        }
    }
}
