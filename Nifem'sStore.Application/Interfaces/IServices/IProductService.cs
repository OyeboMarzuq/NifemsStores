using NifemsStores.Application.DTOs;
using NifemsStores.Application.DTOs.ProductDTO;
using NifemsStores.Application.Common.Response;
using NifemsStores.Domain.Entities;

namespace NifemsStores.Application.Interfaces.IServices
{
    public interface IProductService
    {
        Task<BaseResponse<CreateProductRequestDto>> CreateProduct(CreateProductRequestDto dto, string performedBy);
        Task<BaseResponse<UpdateProductDto>> UpdateProduct(UpdateProductDto dto, string performedBy);
        Task<BaseResponse<string>> DeleteProduct(Guid productId, string performedBy);
        Task<BaseResponse<PaginatedResponse<ProductDto>>> GetAllProducts(ProductFilterRequestDto filter);
        Task<BaseResponse<ProductDto>> GetProductById(Guid productId);
    }
}
