using NifemsStore.Domain.Entities;
using NifemsStore.Domain.Enum;

namespace NifemsStores.Domain.Entities
{
    public class Inventory
    {
        public string ProductId { get; set; } = default!;
        public Product Product { get; set; } = default!;
        public int TotalPiecesAvailable { get; set; }
        public DateTime StockDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string BatchNumber { get; set; } = default!;
        public string? Remark { get; set; }
        public StockStatus StockStatus { get; set; }
    }
}
