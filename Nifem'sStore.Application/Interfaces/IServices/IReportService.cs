using NifemsStore.Application.DTOs.ReportDTO;
using NifemsStores.Application.Common.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.Interfaces.IServices
{
    public interface IReportService
    {
        Task<BaseResponse<ReportDto>> CreateReport(CreateReportDto dto, Guid vendorId);
        Task<BaseResponse<List<ReportDto>>> GetMyReports(Guid vendorId);
        Task<BaseResponse<ReportDto>> GetReportById(Guid reportId, Guid vendorId, bool isAdmin);
        Task<BaseResponse<string>> DeleteReport(Guid reportId, Guid vendorId, bool isAdmin);
    }
}
