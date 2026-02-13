using Microsoft.AspNetCore.Http;
using NifemsStores.Domain.Enum;
using System.Text.Json.Serialization;

namespace NifemsStores.Application.DTOs
{
    public class AdminRegistrationDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("avatar")]
        public IFormFile Avatar { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("surname")]
        public string Surname { get; set; }

        [JsonPropertyName("gender")]
        public Gender Gender { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("nationality")]
        public string Nationality { get; set; }
    }
}
