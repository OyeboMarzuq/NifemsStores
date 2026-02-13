using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.DTOs
{
    public class ProductFilterRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }

        public string? Search { get; set; }
    }
}
