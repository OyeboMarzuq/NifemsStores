using NifemsStores.Domain.Enum;
using NifemsStores.Domain.Entities;

public class Report : BaseEntity
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
    public ReportType ReportType { get; set; }
    public string? FilePath { get; set; }
    public string ReportData { get; set; } = default!;
}
