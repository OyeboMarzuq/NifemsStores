using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NifemsStore.Application.DTOs.ReportDTO;
using NifemsStore.Application.Interfaces.IServices;
using NifemsStores.Application.Common.Response;
using NifemsStores.Domain.Entities;
using NifemsStores.Persistence.Context;

namespace NifemsStores.Persistence.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportService> _logger;

        public ReportService(ApplicationDbContext context, ILogger<ReportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BaseResponse<ReportDto>> CreateReport(CreateReportDto dto, Guid vendorId)
        {
            try
            {
                var report = new Report
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    ReportType = dto.ReportType,
                    VendorId = vendorId,
                    FilePath = dto.FilePath,
                    ReportData = dto.ReportData,
                    GeneratedDate = DateTime.UtcNow
                };

                await _context.Reports.AddAsync(report);
                await _context.SaveChangesAsync();

                var response = new ReportDto
                {
                    ReportId = report.Id,
                    Title = report.Title,
                    Description = report.Description,
                    ReportType = report.ReportType,
                    VendorId = report.VendorId,
                    FilePath = report.FilePath,
                    ReportData = report.ReportData,
                    GeneratedDate = report.GeneratedDate
                };

                return BaseResponse<ReportDto>.Succes(response, "Report created successfully", 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating report");
                return BaseResponse<ReportDto>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<List<ReportDto>>> GetMyReports(Guid vendorId)
        {
            try
            {
                var reports = await _context.Reports
                    .Where(x => x.VendorId == vendorId)
                    .OrderByDescending(x => x.GeneratedDate)
                    .ToListAsync();

                var response = reports.Select(r => new ReportDto
                {
                    ReportId = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    ReportType = r.ReportType,
                    VendorId = r.VendorId,
                    GeneratedDate = r.GeneratedDate,
                    FilePath = r.FilePath,
                    ReportData = r.ReportData
                }).ToList();

                return BaseResponse<List<ReportDto>>.Succes(response, "Reports retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reports");
                return BaseResponse<List<ReportDto>>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<ReportDto>> GetReportById(Guid reportId, Guid vendorId, bool isAdmin)
        {
            try
            {
                var report = await _context.Reports.FirstOrDefaultAsync(x => x.Id == reportId);

                if (report == null)
                    return BaseResponse<ReportDto>.Failure("Report not found", statusCode: 404);

                if (!isAdmin && report.VendorId != vendorId)
                    return BaseResponse<ReportDto>.Failure("Unauthorized access", statusCode: 403);

                var response = new ReportDto
                {
                    ReportId = report.Id,
                    Title = report.Title,
                    Description = report.Description,
                    ReportType = report.ReportType,
                    VendorId = report.VendorId,
                    GeneratedDate = report.GeneratedDate,
                    FilePath = report.FilePath,
                    ReportData = report.ReportData
                };

                return BaseResponse<ReportDto>.Succes(response, "Report retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report");
                return BaseResponse<ReportDto>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<string>> DeleteReport(Guid reportId, Guid vendorId, bool isAdmin)
        {
            try
            {
                var report = await _context.Reports.FirstOrDefaultAsync(x => x.Id == reportId);

                if (report == null)
                    return BaseResponse<string>.Failure("Report not found", statusCode: 404);

                if (!isAdmin && report.VendorId != vendorId)
                    return BaseResponse<string>.Failure("Unauthorized access", statusCode: 403);

                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();

                return BaseResponse<string>.Succes("Report deleted successfully", "Deleted", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting report");
                return BaseResponse<string>.Failure("Something went wrong", statusCode: 500);
            }
        }
    }
}