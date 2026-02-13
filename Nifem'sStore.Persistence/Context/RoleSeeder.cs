using NifemsStores.Domain.Enum;
using Microsoft.AspNetCore.Identity;

namespace NifemsStores.Persistence.Context
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = new[] { Role.Admin, Role.SuperAdmin, Role.User };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
            }
        }
    }
}
