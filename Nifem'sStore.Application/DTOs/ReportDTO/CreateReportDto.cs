using NifemsStore.Domain.Enum;

namespace NifemsStore.Application.DTOs.ReportDTO
{
    public class CreateReportDto
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public ReportType ReportType { get; set; }
        public string ReportData { get; set; } = default!;
        public string? FilePath { get; set; }
    }
}
