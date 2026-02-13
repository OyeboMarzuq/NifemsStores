using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.DTOs.ReportDTO
{
    public class PurchaseReportDto
    {
        public Guid PurchaseReportId { get; set; }
        public Guid VendorId { get; set; }
        public int TotalPurchases { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime? PurchaseDate { get; set; }
    }
}
