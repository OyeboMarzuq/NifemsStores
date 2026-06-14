using Microsoft.EntityFrameworkCore;
using NifemsStore.Application.Interfaces.IRepository;
using NifemsStores.Domain.Entities;
using NifemsStores.Persistence.Context;

namespace NifemsStores.Persistence.Repositories
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly ApplicationDbContext _context;

        public ReceiptRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task GenerateReceiptAsync(Receipt receipt)
        {
            await _context.Receipts.AddAsync(receipt);
            await _context.SaveChangesAsync();
        }

        public async Task<Receipt?> GetByIdAsync(Guid id)
        {
            return await _context.Receipts
                .Include(r => r.ReceiptItems)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}