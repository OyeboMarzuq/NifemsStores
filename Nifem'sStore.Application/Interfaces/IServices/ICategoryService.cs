using NifemsStore.Application.DTOs.CategoryDTO;
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
    public interface ICategoryService
    {
        Task<BaseResponse<CategoryDto>> CreateCategory(CreateCategoryRequestDto dto, Guid vendorId, string performedBy);

        Task<BaseResponse<CategoryDto>> UpdateCategory(Guid categoryId, UpdateCategoryDto dto, Guid vendorId, string performedBy);

        Task<BaseResponse<string>> DeleteCategory(Guid categoryId, Guid vendorId, string performedBy);

        Task<BaseResponse<List<CategoryDto>>> GetAllCategories(Guid vendorId);

        Task<BaseResponse<CategoryDto>> GetCategoryById(Guid categoryId, Guid vendorId);
    }
}
