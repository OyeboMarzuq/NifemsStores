using NifemsStores.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.Interfaces.IServices
{
    public interface IAuditLogService
    {
        Task LogAsync(AuditLog log);
    }
}
