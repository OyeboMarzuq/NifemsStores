using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NifemsStores.Application.DTOs.ReportDTO;
using NifemsStores.Application.Interfaces.IServices;
using NifemsStores.Application.Common.Response;
using NifemsStores.Persistence.Context;

namespace NifemsStores.Persistence.Services
{
    public class PurchaseReportService : IPurchaseReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PurchaseReportService> _logger;

        public PurchaseReportService(ApplicationDbContext context, ILogger<PurchaseReportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BaseResponse<List<PurchaseReportDto>>> GetAllPurchaseReports()
        {
            try
            {
                var reports = await _context.PurchaseReports
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(x => new PurchaseReportDto
                    {
                        PurchaseReportId = x.Id,
                        TotalPurchases = x.TotalPurchases,
                        TotalSpent = x.TotalSpent,
                        TotalCost = x.TotalCost,
                        PurchaseDate = x.PurchaseDate
                    })
                    .ToListAsync();

                return BaseResponse<List<PurchaseReportDto>>.Success(reports, "Purchase reports retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving purchase reports");
                return BaseResponse<List<PurchaseReportDto>>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<PurchaseReportDto>> GetPurchaseReportById(Guid reportId)
        {
            try
            {
                var report = await _context.PurchaseReports.FirstOrDefaultAsync(x => x.Id == reportId);

                if (report == null)
                    return BaseResponse<PurchaseReportDto>.Failure("Purchase report not found", statusCode: 404);

                return BaseResponse<PurchaseReportDto>.Success(new PurchaseReportDto
                {
                    PurchaseReportId = report.Id,
                    TotalPurchases = report.TotalPurchases,
                    TotalSpent = report.TotalSpent,
                    TotalCost = report.TotalCost,
                    PurchaseDate = report.PurchaseDate
                }, "Purchase report retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving purchase report");
                return BaseResponse<PurchaseReportDto>.Failure("Something went wrong", statusCode: 500);
            }
        }
    }
}
