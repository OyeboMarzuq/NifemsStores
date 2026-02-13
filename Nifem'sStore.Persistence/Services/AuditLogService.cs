using Microsoft.Extensions.Logging;
using NifemsStore.Application.Interfaces.IServices;
using NifemsStores.Domain.Entities;
using NifemsStores.Persistence.Context;

namespace NifemsStores.Persistence.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(ApplicationDbContext context, ILogger<AuditLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogAsync(AuditLog log)
        {
            try
            {
                await _context.AuditLogs.AddAsync(log);
                await _context.SaveChangesAsync();

                _logger.LogInformation("AuditLog saved successfully: {Action} on {EntityName} ({EntityId}) by {User}",
                    log.Action, log.EntityName, log.EntityId, log.PerformedBy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save AuditLog");
            }
        }
    }
}
