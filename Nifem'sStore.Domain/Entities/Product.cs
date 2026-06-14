using NifemsStores.Domain.Enum;
using NifemsStores.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Domain.Entities
{
    public class Product : BaseEntity
    {
        public Guid ProductId { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public decimal? UnitPrice { get; set; }

        public StockStatus StockStatus { get; set; }
        
        public int? PackPrice { get; set; }
        public int? TotalPiecesPerPack { get; set; }
        public string? ProductImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;

        public Guid? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public decimal EffectivePrice => UnitPrice ?? (PackPrice + PackPriceMarkup);

        public int QuantityInStock { get; set; }


    }
}
