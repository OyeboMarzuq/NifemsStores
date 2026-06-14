using Microsoft.AspNetCore.Http;
using NifemsStores.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NifemsStores.Application.DTOs
{
    public class UserRegistrationDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;

        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Surname { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Address { get; set; } = default!;
        public Gender Gender { get; set; }

        [JsonPropertyName("avatar")]
        public IFormFile Avatar { get; set; }
    }
}
