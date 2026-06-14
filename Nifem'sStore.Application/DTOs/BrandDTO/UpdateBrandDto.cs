using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Application.DTOs.BrandDTO
{
    public class UpdateBrandDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public IFormFile Avatar { get; set; }
    }
}
