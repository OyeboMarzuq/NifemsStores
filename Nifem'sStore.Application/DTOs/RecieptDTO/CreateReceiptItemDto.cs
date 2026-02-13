using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.DTOs.RecieptDTO
{
    public class CreateReceiptItemDto
    {
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
