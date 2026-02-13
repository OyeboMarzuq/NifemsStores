using NifemsStore.Application.DTOs.CartDTO;
using NifemsStores.Application.Common.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.Interfaces.IServices
{
    public interface ICartService
    {
        Task<BaseResponse<CartDto>> GetMyCart(Guid userId);
        Task<BaseResponse<string>> AddToCart(AddToCartDto dto, Guid userId);
        Task<BaseResponse<string>> UpdateCartItem(UpdateCartItemDto dto, Guid userId);
        Task<BaseResponse<string>> RemoveCartItem(Guid cartItemId, Guid userId);
        Task<BaseResponse<string>> ClearCart(Guid userId);
    }
}
