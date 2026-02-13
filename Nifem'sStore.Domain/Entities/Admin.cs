using NifemsStores.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Domain.Entities
{
    public class Admin : BaseEntity
    {
        public string Email { get; set; }
        public string? Avatar { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
        public string GenderDesc { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public Admin()
        {
            GenderDesc = Gender.ToString();
        }
    }
}
