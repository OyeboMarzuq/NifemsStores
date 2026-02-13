using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NifemsStore.Application.DTOs.RecieptDTO;
using NifemsStore.Application.Interfaces.IServices;
using NifemsStores.Application.Common.Response;
using NifemsStores.Domain.Entities;
using NifemsStores.Persistence.Context;

namespace NifemsStores.Persistence.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReceiptService> _logger;

        public ReceiptService(ApplicationDbContext context, ILogger<ReceiptService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BaseResponse<ReceiptDto>> CreateReceipt(CreateReceiptDto dto, Guid userId, string userName)
        {
            try
            {
                if (dto.Items == null || dto.Items.Count == 0)
                    return BaseResponse<ReceiptDto>.Failure("Receipt must contain at least one item", statusCode: 400);

                var totalAmount = dto.Items.Sum(x => x.Quantity * x.UnitPrice);
                var netAmount = totalAmount - dto.Discount;

                if (netAmount < 0)
                    return BaseResponse<ReceiptDto>.Failure("Discount cannot exceed total amount", statusCode: 400);

                var receipt = new Receipt
                {
                    ReceiptNumber = $"RCPT-{DateTime.UtcNow.Ticks}",
                    TotalAmount = totalAmount,
                    Discount = dto.Discount,
                    NetAmount = netAmount,
                    UserId = userId,
                    UserName = userName,
                    PaymentType = dto.PaymentType,
                    ReceiptItems = dto.Items.Select(x => new ReceiptItem
                    {
                        ProductName = x.ProductName,
                        Quantity = x.Quantity,
                        Description = x.Description,
                        UnitPrice = x.UnitPrice
                    }).ToList()
                };

                await _context.Receipts.AddAsync(receipt);
                await _context.SaveChangesAsync();

                var response = new ReceiptDto
                {
                    ReceiptId = receipt.Id,
                    ReceiptNumber = receipt.ReceiptNumber,
                    TotalAmount = receipt.TotalAmount,
                    Discount = receipt.Discount,
                    NetAmount = receipt.NetAmount,
                    UserName = receipt.UserName,
                    PaymentType = receipt.PaymentType,
                    CreatedAt = receipt.CreatedAt,
                    ReceiptItems = receipt.ReceiptItems.Select(i => new ReceiptItemDto
                    {
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        SubTotal = i.SubTotal
                    }).ToList()
                };

                return BaseResponse<ReceiptDto>.Succes(response, "Receipt created successfully", 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating receipt");
                return BaseResponse<ReceiptDto>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<List<ReceiptDto>>> GetMyReceipts(Guid userId)
        {
            try
            {
                var receipts = await _context.Receipts
                    .Include(x => x.ReceiptItems)
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                var response = receipts.Select(r => new ReceiptDto
                {
                    ReceiptId = r.Id,
                    ReceiptNumber = r.ReceiptNumber,
                    TotalAmount = r.TotalAmount,
                    Discount = r.Discount,
                    NetAmount = r.NetAmount,
                    UserName = r.UserName,
                    PaymentType = r.PaymentType,
                    CreatedAt = r.CreatedAt,
                    ReceiptItems = r.ReceiptItems.Select(i => new ReceiptItemDto
                    {
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        SubTotal = i.SubTotal
                    }).ToList()
                }).ToList();

                return BaseResponse<List<ReceiptDto>>.Succes(response, "Receipts retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving receipts");
                return BaseResponse<List<ReceiptDto>>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<ReceiptDto>> GetReceiptById(Guid receiptId, Guid userId, bool isAdmin)
        {
            try
            {
                var receipt = await _context.Receipts
                    .Include(x => x.ReceiptItems)
                    .FirstOrDefaultAsync(x => x.Id == receiptId);

                if (receipt == null)
                    return BaseResponse<ReceiptDto>.Failure("Receipt not found", statusCode: 404);

                if (!isAdmin && receipt.UserId != userId)
                    return BaseResponse<ReceiptDto>.Failure("Unauthorized access", statusCode: 403);

                var response = new ReceiptDto
                {
                    ReceiptId = receipt.Id,
                    ReceiptNumber = receipt.ReceiptNumber,
                    TotalAmount = receipt.TotalAmount,
                    Discount = receipt.Discount,
                    NetAmount = receipt.NetAmount,
                    UserName = receipt.UserName,
                    PaymentType = receipt.PaymentType,
                    CreatedAt = receipt.CreatedAt,
                    ReceiptItems = receipt.ReceiptItems.Select(i => new ReceiptItemDto
                    {
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        SubTotal = i.SubTotal
                    }).ToList()
                };

                return BaseResponse<ReceiptDto>.Succes(response, "Receipt retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving receipt");
                return BaseResponse<ReceiptDto>.Failure("Something went wrong", statusCode: 500);
            }
        }
    }
}
