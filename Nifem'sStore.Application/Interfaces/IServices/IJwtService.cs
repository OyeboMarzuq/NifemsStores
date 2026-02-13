using NifemsStores.Domain.Entities;

namespace NifemsStores.Application.Interfaces.IServices
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
    }
}
