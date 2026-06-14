using NifemsStore.Application.DTOs.ProductDTO;
using NifemsStores.Application.DTOs.RecieptDTO;

namespace NifemsStores.Application.Interfaces.IServices
{
    public interface IReceiptService
    {
        Task<(ReceiptDto Receipt, byte[] PdfBytes)>
            GenerateFromProductSaleAsync(ProductSaleDto sale);
    }
}