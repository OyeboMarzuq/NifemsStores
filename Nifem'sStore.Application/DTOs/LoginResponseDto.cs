using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Avatar { get; set; }
    }
}
