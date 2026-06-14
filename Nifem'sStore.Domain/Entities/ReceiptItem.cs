using NifemsStores.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace NifemsStores.Domain.Entities
{
    public class ReceiptItem : BaseEntity
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        [NotMapped]
        public decimal SubTotal => Quantity * UnitPrice;
        public Guid ReceiptId { get; set; }
        public Receipt? Receipt { get; set; }
    }
}
