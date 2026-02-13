using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NifemsStore.Application.DTOs.CartDTO;
using NifemsStore.Application.Interfaces.IServices;
using System.Security.Claims;

namespace NifemsStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet("my-cart")]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = GetUserId();
            var response = await _cartService.GetMyCart(userId);
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = GetUserId();
            var response = await _cartService.AddToCart(dto, userId);
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpPut("update-item")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto dto)
        {
            var userId = GetUserId();
            var response = await _cartService.UpdateCartItem(dto, userId);
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpDelete("remove-item/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(Guid cartItemId)
        {
            var userId = GetUserId();
            var response = await _cartService.RemoveCartItem(cartItemId, userId);
            return StatusCode(response.StatusCode ?? 500, response);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            var response = await _cartService.ClearCart(userId);
            return StatusCode(response.StatusCode ?? 500, response);
        }
    }
}
