using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.DTOs.BrandDTO
{
    public class CreateBrandRequestDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
