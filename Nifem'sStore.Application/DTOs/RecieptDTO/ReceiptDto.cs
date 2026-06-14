using NifemsStore.Domain.Enum;

namespace NifemsStores.Application.DTOs.RecieptDTO
{
    public class ReceiptDto
    {
        public Guid ReceiptId { get; set; }
        public string ReceiptNumber { get; set; } = default!;
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; }
        public string? UserName { get; set; }
        public PaymentType PaymentType { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<ReceiptItemDto> ReceiptItems { get; set; } = new();
    }
}
