using NifemsStore.Domain.Enum;
using NifemsStores.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Domain.Entities
{
    public class Product : BaseEntity
    {
        public Guid ProductId { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public decimal PricePerPack { get; set; }
        public decimal PackPriceMarkup { get; set; }

        public decimal? UnitPrice { get; set; }

        public StockStatus StockStatus { get; set; }

        public string? ProductImageUrl { get; set; }

        public int TotalItemInPack { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;

        public Guid? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public decimal EffectivePrice => UnitPrice ?? (PricePerPack + PackPriceMarkup);

        public Guid Vendor { get; set; }
        public DateTime CreatedAt { get; set; }

        public int QuantityInStock { get; set; }

        //This Will be used to fetch the vendor details when needed And Will be on the product searched to indicate who is selling it
        //This Will Be Used In The "Featured Goods" Dashboard

        public string UserName { get; set; } = default!;
    }
}
