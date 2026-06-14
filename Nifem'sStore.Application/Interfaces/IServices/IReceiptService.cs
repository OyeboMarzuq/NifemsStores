using NifemsStore.Application.DTOs.ProductDTO;
using NifemsStore.Application.DTOs.RecieptDTO;

namespace NifemsStore.Application.Interfaces.IServices
{
    public interface IReceiptService
    {
        Task<(ReceiptDto Receipt, byte[] PdfBytes)>
            GenerateFromProductSaleAsync(ProductSaleDto sale);
    }
}