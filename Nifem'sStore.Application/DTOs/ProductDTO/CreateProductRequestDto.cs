using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.DTOs.ProductDTO
{
    public class CreateProductRequestDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public decimal PricePerPack { get; set; }
        public decimal PackPriceMarkup { get; set; }

        public int TotalItemInPack { get; set; }

        public Guid CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public string? ProductImageUrl { get; set; }
        public Guid Vendor { get; set; }
        public string UserName { get; set; } = default!;

        public int QuantityInStock { get; set; }
    }
}
