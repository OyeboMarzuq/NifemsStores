using NifemsStore.Application.DTOs.BrandDTO;
using NifemsStores.Application.Common.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.Interfaces.IServices
{
    public interface IBrandService
    {
        Task<BaseResponse<BrandDto>> CreateBrand(CreateBrandRequestDto request);
        Task<BaseResponse<List<BrandDto>>> GetAllBrands();
        Task<BaseResponse<BrandDto>> GetBrandById(Guid id);
        Task<BaseResponse<BrandDto>> UpdateBrand(UpdateBrandDto request);
        Task<BaseResponse<bool>> DeleteBrand(Guid id);
    }
}
