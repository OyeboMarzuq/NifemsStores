
using Microsoft.AspNetCore.Identity;
using NifemsStores.Domain.Entities;
using NifemsStores.Domain.Enum;

namespace NifemsStores.Persistence.Context
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public DataSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAdminAsync()
        {
            var adminEmail = "MorizuqOyebo@gmail.com";
            var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "Marzuq",
                    Email = adminEmail,
                    Surname = "Oyebo",
                    LastName = "Bamidele",
                    FirstName = "MarYZuq",
                    Gender = Gender.Male,
                    DateOfBirth = new DateOnly(2006, 5, 2),
                    Nationality = "Nigeria",
                    Address = "Admin HQ"
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin@123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");

                    var admin = new Admin
                    {
                        FirstName = "yeeshamhi@gmail.com",
                        Surname = "Oyebo",
                        LastName = "Aishat",
                        Email = adminEmail,
                        PhoneNumber = "09015509551",
                        Address = "Admin HQ",
                        ApplicationUserId = adminUser.Id
                    };

                    _context.Admins.Add(admin);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
