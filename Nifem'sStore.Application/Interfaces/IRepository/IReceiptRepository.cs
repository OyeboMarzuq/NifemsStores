using NifemsStores.Domain.Entities;

namespace NifemsStore.Application.Interfaces.IRepository
{
    public interface IReceiptRepository
    {
        Task GenerateReceiptAsync(Receipt receipt);
        Task<Receipt?> GetByIdAsync(Guid id);
    }
}