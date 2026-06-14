using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NifemsStores.Application.DTOs.CartDTO;
using NifemsStores.Application.Interfaces.IServices;
using NifemsStores.Domain.Entities;
using NifemsStores.Application.Common.Response;
using NifemsStores.Domain.Entities;
using NifemsStores.Persistence.Context;

namespace NifemsStores.Persistence.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartService> _logger;

        public CartService(ApplicationDbContext context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BaseResponse<CartDto>> GetMyCart(Guid userId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(x => x.CartItems)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (cart == null)
                    return BaseResponse<CartDto>.Failure("Cart is empty", statusCode: 404);

                var response = new CartDto
                {
                    CartId = cart.CartId,
                    UserId = cart.UserId,
                    CartItems = cart.CartItems.Select(ci => new CartItemDto
                    {
                        CartItemId = ci.CartItemId,
                        ProductId = ci.ProductId,
                        ProductName = ci.Product.Name,
                        UnitPrice = ci.Product.UnitPrice ?? ci.Product.PricePerPack,
                        Quantity = ci.Quantity,
                        SubTotal = ci.Quantity * (ci.Product.UnitPrice ?? ci.Product.PricePerPack)
                    }).ToList()
                };

                response.TotalAmount = response.CartItems.Sum(x => x.SubTotal);

                return BaseResponse<CartDto>.Success(response, "Cart retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart");
                return BaseResponse<CartDto>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<string>> AddToCart(AddToCartDto dto, Guid userId)
        {
            try
            {
                if (dto.Quantity <= 0)
                    return BaseResponse<string>.Failure("Quantity must be greater than zero", statusCode: 400);

                var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == dto.ProductId);

                if (product == null)
                    return BaseResponse<string>.Failure("Product not found", statusCode: 404);

                if (dto.Quantity > product.QuantityInStock)
                    return BaseResponse<string>.Failure($"Only {product.QuantityInStock} unit(s) available in stock", statusCode: 400);

                var cart = await _context.Carts
                    .Include(x => x.CartItems)
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        CartId = Guid.NewGuid(),
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _context.Carts.AddAsync(cart);
                    await _context.SaveChangesAsync();
                }

                var existingItem = cart.CartItems.FirstOrDefault(x => x.ProductId == dto.ProductId);

                if (existingItem != null)
                {
                    var newQty = existingItem.Quantity + dto.Quantity;
                    if (newQty > product.QuantityInStock)
                        return BaseResponse<string>.Failure($"Cannot add {dto.Quantity} more. Only {product.QuantityInStock - existingItem.Quantity} additional unit(s) available", statusCode: 400);
                    existingItem.Quantity = newQty;
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        CartItemId = Guid.NewGuid(),
                        CartId = cart.CartId,
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity
                    };

                    await _context.CartItems.AddAsync(cartItem);
                }

                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return BaseResponse<string>.Success("Added to cart successfully", "Success", 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                return BaseResponse<string>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<string>> UpdateCartItem(UpdateCartItemDto dto, Guid userId)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .Include(x => x.Cart)
                    .FirstOrDefaultAsync(x => x.CartItemId == dto.CartItemId);

                if (cartItem == null)
                    return BaseResponse<string>.Failure("Cart item not found", statusCode: 404);

                if (cartItem.Cart.UserId != userId)
                    return BaseResponse<string>.Failure("Unauthorized access", statusCode: 403);

                if (dto.Quantity <= 0)
                    return BaseResponse<string>.Failure("Quantity must be greater than zero", statusCode: 400);

                cartItem.Quantity = dto.Quantity;
                cartItem.Cart.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return BaseResponse<string>.Success("Cart item updated successfully", "Updated", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item");
                return BaseResponse<string>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<string>> RemoveCartItem(Guid cartItemId, Guid userId)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .Include(x => x.Cart)
                    .FirstOrDefaultAsync(x => x.CartItemId == cartItemId);

                if (cartItem == null)
                    return BaseResponse<string>.Failure("Cart item not found", statusCode: 404);

                if (cartItem.Cart.UserId != userId)
                    return BaseResponse<string>.Failure("Unauthorized access", statusCode: 403);

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                return BaseResponse<string>.Success("Cart item removed successfully", "Deleted", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item");
                return BaseResponse<string>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<string>> ClearCart(Guid userId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(x => x.CartItems)
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (cart == null)
                    return BaseResponse<string>.Failure("Cart not found", statusCode: 404);

                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();

                return BaseResponse<string>.Success("Cart cleared successfully", "Cleared", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                return BaseResponse<string>.Failure("Something went wrong", statusCode: 500);
            }
        }
    }
}
