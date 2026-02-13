using Microsoft.AspNetCore.Identity;
using NifemsStores.Domain.Enum;

namespace NifemsStores.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? Avatar { get; set; }
        public string Surname { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public Gender? Gender { get; set; }
        public string? Address { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public Customer? Customer { get; set; }
        public Admin? Admin { get; set; }
    }
}
