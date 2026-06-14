using NifemsStores.Domain.Enum;

namespace NifemsStores.Application.DTOs.ReportDTO
{
    public class ReportDto
    {
        public Guid ReportId { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public ReportType ReportType { get; set; }
            public DateTime GeneratedDate { get; set; }
        public string? FilePath { get; set; }
        public string ReportData { get; set; } = default!;
    }
}
