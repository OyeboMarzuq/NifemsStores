using NifemsStore.Application.DTOs.AdvertisementDTO;
using NifemsStores.Application.Common.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Application.Interfaces.IServices
{
    public interface IAdvertisementService
    {
        Task<BaseResponse<AdvertisementDto>> CreateAsync(CreateAdvertisementDto dto, string performedBy);
        Task<BaseResponse<AdvertisementDto>> UpdateAsync(UpdateAdvertisementDto dto, string performedBy);
        Task<BaseResponse<string>> DeleteAsync(Guid id, string performedBy);
    }
}
