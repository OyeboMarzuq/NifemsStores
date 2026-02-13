using NifemsStore.Application.DTOs;
using NifemsStore.Application.DTOs.ProductDTO;
using NifemsStore.Domain.Entities;
using NifemsStores.Application.Common.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.Interfaces.IServices
{
    public interface IProductService
    {
        Task<BaseResponse<CreateProductRequestDto>> CreateProduct(CreateProductRequestDto dto, Guid vendorId, string userName);
        Task<BaseResponse<UpdateProductDto>> UpdateProduct(UpdateProductDto dto, Guid vendorId, bool isAdmin);
        Task<BaseResponse<string>> DeleteProduct(Guid productId, Guid vendorId, bool isAdmin);
        Task<BaseResponse<PaginatedResponse<ProductDto>>> GetAllProducts(ProductFilterRequestDto filter, Guid vendorId, bool isAdmin);
        Task<BaseResponse<ProductDto>> GetProductById(Guid productId);
    }
}
