using NifemsStores.Application.DTOs.ReportDTO;
using NifemsStores.Application.Common.Response;

namespace NifemsStores.Application.Interfaces.IServices
{
    public interface IPurchaseReportService
    {
        Task<BaseResponse<List<PurchaseReportDto>>> GetAllPurchaseReports();
        Task<BaseResponse<PurchaseReportDto>> GetPurchaseReportById(Guid reportId);
    }
}
