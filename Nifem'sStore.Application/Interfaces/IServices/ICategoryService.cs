using NifemsStores.Application.DTOs.CategoryDTO;
using NifemsStores.Application.Common.Response;

namespace NifemsStores.Application.Interfaces.IServices
{
    public interface ICategoryService
    {
        Task<BaseResponse<CategoryDto>> CreateCategory(CreateCategoryRequestDto dto, string performedBy);
        Task<BaseResponse<CategoryDto>> UpdateCategory(Guid categoryId, UpdateCategoryDto dto, string performedBy);
        Task<BaseResponse<string>> DeleteCategory(Guid categoryId, string performedBy);
        Task<BaseResponse<List<CategoryDto>>> GetAllCategories();
        Task<BaseResponse<CategoryDto>> GetCategoryById(Guid categoryId);
    }
}
