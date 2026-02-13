using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.DTOs.CategoryDTO
{
    public class CreateCategoryRequestDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
