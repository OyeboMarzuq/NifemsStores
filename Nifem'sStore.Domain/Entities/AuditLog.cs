using System;

namespace NifemsStores.Domain.Entities
{
    public class AuditLog
    {
        public Guid AuditLogId { get; set; } = Guid.NewGuid();

        public string Action { get; set; } = default!;
        public string EntityName { get; set; } = default!;
        public string EntityId { get; set; } = default!;

        public string? OldValues { get; set; }
        public string? NewValues { get; set; }

        public string PerformedBy { get; set; } = default!;
        public string Role { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
