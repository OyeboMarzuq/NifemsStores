using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NifemsStore.Application.DTOs.ProductDTO;
using NifemsStore.Application.DTOs.RecieptDTO;
using NifemsStore.Application.Interfaces.IRepository;
using NifemsStore.Application.Interfaces.IServices;
using NifemsStores.Application.Common.Response;
using NifemsStores.Domain.Entities;
using NifemsStores.Persistence.Context;
using QuestPDF.Fluent;

namespace NifemsStores.Persistence.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly ApplicationDbContext _context;
        private readonly IReceiptRepository _receiptRepository;

        public ReceiptService(ApplicationDbContext context, IReceiptRepository receiptRepository)
        {
            _context = context;
            _receiptRepository = receiptRepository;
        }

        public async Task<(ReceiptDto Receipt, byte[] PdfBytes)> GenerateFromProductSaleAsync(ProductSaleDto sale)
        {
            if (sale == null || sale.ProductSaleItemDto == null || !sale.ProductSaleItemDto.Any())
                throw new ArgumentException("Sale or Sale Items are empty.");

            var receiptNumber = $"ASR-{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow.Ticks.ToString()[^6..]}";

            var receiptEntity = new Receipt
            {
                ReceiptNumber = receiptNumber,
                UserId = Guid.Empty,
                UserName = sale.CustomerName,
                Discount = sale.Discount ?? 0,
                TotalAmount = sale.TotalAmount,
                NetAmount = sale.TotalAmount - (sale.Discount ?? 0),
                PaymentType = sale.PaymentMethod,
                CreatedAt = DateTime.UtcNow,

                ReceiptItems = sale.ProductSaleItemDto.Select(item => new ReceiptItem
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            await _receiptRepository.GenerateReceiptAsync(receiptEntity);
            var receiptDto = new ReceiptDto
            {
                ReceiptId = receiptEntity.Id,
                ReceiptNumber = receiptEntity.ReceiptNumber,
                UserName = receiptEntity.UserName,
                TotalAmount = receiptEntity.TotalAmount,
                NetAmount = receiptEntity.NetAmount,
                Discount = receiptEntity.Discount,
                PaymentType = receiptEntity.PaymentType,
                ReceiptItems = receiptEntity.ReceiptItems.Select(r => new ReceiptItemDto
                {
                    ProductName = r.ProductName,
                    Quantity = r.Quantity,
                    UnitPrice = r.UnitPrice,
                    SubTotal = r.SubTotal
                }).ToList()
            };

            var qrImageData = GenerateQrCodeImage(receiptNumber);
            var pdfBytes = GeneratePdfReceipt(receiptEntity, qrImageData);
            return (receiptDto, pdfBytes);
        }

        private byte[] GenerateQrCodeImage(string receiptNumber)
        {
            throw new NotImplementedException();
        }

        private byte[] GeneratePdfReceipt(Receipt receipt, byte[] qrImageData)
        {
            try
            {
                var pdf = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(20);

                        page.Content().Column(column =>
                        {
                            column.Item().AlignCenter().Text("NIFEM'S STORE").Bold().FontSize(16);
                            column.Item().AlignCenter().Text("SALES RECEIPT").Bold();

                            column.Item().PaddingVertical(10);

                            column.Item().Text($"Customer: {receipt.UserName ?? "Walk-in Customer"}");
                            column.Item().Text($"Payment: {receipt.PaymentType}");
                            column.Item().Text($"Receipt #: {receipt.ReceiptNumber}");
                            column.Item().Text($"Date: {receipt.CreatedAt:MMM dd, yyyy HH:mm}");

                            column.Item().PaddingVertical(10);

                            int serial = 1;
                            foreach (var item in receipt.ReceiptItems)
                            {
                                column.Item().Text($"{serial}. {item.ProductName}");
                                column.Item().Text($"Qty: {item.Quantity} | Unit: ₦{item.UnitPrice:N2} | Total: ₦{item.SubTotal:N2}");
                                serial++;
                            }

                            column.Item().PaddingVertical(10);

                            if (receipt.Discount > 0)
                            {
                                var subtotal = receipt.ReceiptItems.Sum(x => x.SubTotal);

                                column.Item().Text($"Subtotal: ₦{subtotal:N2}");
                                column.Item().Text($"Discount: -₦{receipt.Discount:N2}");
                            }

                            column.Item().Text($"TOTAL: ₦{receipt.NetAmount:N2}").Bold();

                            column.Item().PaddingVertical(10);

                            if (qrImageData != null && qrImageData.Length > 0)
                                column.Item().Height(80).Image(qrImageData);

                            column.Item().AlignCenter().Text("Thank you for your purchase!");
                        });
                    });
                });

                return pdf.GeneratePdf();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to generate PDF receipt.", ex);
            }
        }

        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            if (maxLength <= 3)
                return text.Length <= maxLength ? text : text.Substring(0, maxLength);

            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 3) + "...";
        }
    }
}
