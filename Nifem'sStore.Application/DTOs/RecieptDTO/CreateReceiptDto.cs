using NifemsStore.Domain.Enum;

namespace NifemsStore.Application.DTOs.RecieptDTO
{
    public class CreateReceiptDto
    {
        public decimal Discount { get; set; }
        public PaymentType PaymentType { get; set; }

        public List<CreateReceiptItemDto> Items { get; set; } = new();
    }
}
