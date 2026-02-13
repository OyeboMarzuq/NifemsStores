using NifemsStore.Domain.Enum;
using NifemsStores.Domain.Entities;

public class Report : BaseEntity
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
    public ReportType ReportType { get; set; }
    public Guid VendorId { get; set; }
    public Customer Vendor { get; set; } = default!;
    public string? FilePath { get; set; }
    public string ReportData { get; set; } = default!;
}
