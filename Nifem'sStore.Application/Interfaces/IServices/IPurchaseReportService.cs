using NifemsStore.Application.DTOs.ReportDTO;
using NifemsStores.Application.Common.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.Interfaces.IServices
{
    public interface IPurchaseReportService
    {
        Task<BaseResponse<List<PurchaseReportDto>>> GetVendorPurchaseReports(Guid vendorId, bool isAdmin);
        Task<BaseResponse<PurchaseReportDto>> GetPurchaseReportById(Guid reportId, Guid vendorId, bool isAdmin);
    }
}
