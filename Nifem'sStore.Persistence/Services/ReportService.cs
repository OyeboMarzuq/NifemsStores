using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NifemsStores.Application.DTOs.ReportDTO;
using NifemsStores.Application.Interfaces.IServices;
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

        public async Task<BaseResponse<ReportDto>> CreateReport(CreateReportDto dto)
        {
            try
            {
                var report = new Report
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    ReportType = dto.ReportType,
                    FilePath = dto.FilePath,
                    ReportData = dto.ReportData,
                    GeneratedDate = DateTime.UtcNow
                };

                await _context.Reports.AddAsync(report);
                await _context.SaveChangesAsync();

                return BaseResponse<ReportDto>.Success(MapToDto(report), "Report created successfully", 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating report");
                return BaseResponse<ReportDto>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<List<ReportDto>>> GetAllReports()
        {
            try
            {
                var reports = await _context.Reports
                    .OrderByDescending(x => x.GeneratedDate)
                    .ToListAsync();

                return BaseResponse<List<ReportDto>>.Success(
                    reports.Select(MapToDto).ToList(),
                    "Reports retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reports");
                return BaseResponse<List<ReportDto>>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<ReportDto>> GetReportById(Guid reportId)
        {
            try
            {
                var report = await _context.Reports.FirstOrDefaultAsync(x => x.Id == reportId);
                if (report == null)
                    return BaseResponse<ReportDto>.Failure("Report not found", statusCode: 404);

                return BaseResponse<ReportDto>.Success(MapToDto(report), "Report retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report");
                return BaseResponse<ReportDto>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<string>> DeleteReport(Guid reportId)
        {
            try
            {
                var report = await _context.Reports.FirstOrDefaultAsync(x => x.Id == reportId);
                if (report == null)
                    return BaseResponse<string>.Failure("Report not found", statusCode: 404);

                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();

                return BaseResponse<string>.Success("Deleted", "Report deleted successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting report");
                return BaseResponse<string>.Failure("Something went wrong", statusCode: 500);
            }
        }

        private static ReportDto MapToDto(Report r) => new()
        {
            ReportId = r.Id,
            Title = r.Title,
            Description = r.Description,
            ReportType = r.ReportType,
            GeneratedDate = r.GeneratedDate,
            FilePath = r.FilePath,
            ReportData = r.ReportData
        };
    }
}
