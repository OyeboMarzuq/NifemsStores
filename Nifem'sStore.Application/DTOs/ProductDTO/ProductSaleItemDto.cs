using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.DTOs.ProductDTO
{
    public class ProductSaleItemDto
    {
        public string ProductId { get; set; } = default!;
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
