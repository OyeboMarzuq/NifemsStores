using NifemsStore.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;


namespace NifemsStores.Domain.Entities
{
    public class Receipt : BaseEntity
    {
        public string ReceiptNumber { get; set; } = default!;

        [Column(TypeName = "money")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "money")]
        public decimal Discount { get; set; }

        [Column(TypeName = "money")]
        public decimal NetAmount { get; set; }

        public Guid UserId { get; set; }
        public string? UserName { get; set; }

        public PaymentType PaymentType { get; set; }

        public ICollection<ReceiptItem> ReceiptItems { get; set; } = [];
        public DateTime CreatedAt { get; set; }
    }
}
