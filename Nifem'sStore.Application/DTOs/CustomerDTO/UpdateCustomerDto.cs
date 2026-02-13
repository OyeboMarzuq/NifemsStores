using Microsoft.AspNetCore.Http;
using NifemsStores.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.DTOs.CustomerDTO
{
    public class UpdateCustomerDto
    {
        public Guid Id { get; set; }
        public string Surname { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        public IFormFile? Avatar { get; set; }

        public string PhoneNumber { get; set; } = default!;
        public string Address { get; set; } = default!;

        public Gender Gender { get; set; }
    }
}
