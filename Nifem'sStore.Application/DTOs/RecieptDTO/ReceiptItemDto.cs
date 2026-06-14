using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Application.DTOs.RecieptDTO
{
    public class ReceiptItemDto
    {
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
