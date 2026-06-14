using NifemsStores.Application.DTOs.ReportDTO;
using NifemsStores.Application.Common.Response;

namespace NifemsStores.Application.Interfaces.IServices
{
    public interface IReportService
    {
        Task<BaseResponse<ReportDto>> CreateReport(CreateReportDto dto);
        Task<BaseResponse<List<ReportDto>>> GetAllReports();
        Task<BaseResponse<ReportDto>> GetReportById(Guid reportId);
        Task<BaseResponse<string>> DeleteReport(Guid reportId);
    }
}
