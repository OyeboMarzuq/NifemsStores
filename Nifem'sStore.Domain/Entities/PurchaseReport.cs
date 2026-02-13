using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Domain.Entities
{
    public class PurchaseReport : BaseEntity
    {
        public Guid VendorId { get; set; }
        public int TotalPurchases { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
