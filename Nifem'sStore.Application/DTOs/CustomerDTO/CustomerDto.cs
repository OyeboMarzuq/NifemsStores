using NifemsStores.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.DTOs.UserDTO
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }

        public string Email { get; set; } = default!;
        public string Surname { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        public string? Avatar { get; set; }

        public string PhoneNumber { get; set; } = default!;
        public string Address { get; set; } = default!;

        public Gender Gender { get; set; }
        public string GenderDesc { get; set; } = default!;

        public DateTime DateCreated { get; set; }
    }
}
