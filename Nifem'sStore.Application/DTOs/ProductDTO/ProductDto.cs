using NifemsStores.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Application.DTOs.ProductDTO
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public decimal PricePerPack { get; set; }
        public decimal PackPriceMarkup { get; set; }

        public decimal? UnitPrice { get; set; }
        public decimal EffectivePrice { get; set; }

        public StockStatus StockStatus { get; set; }

        public int TotalItemInPack { get; set; }
        public int QuantityInStock { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;

        public Guid? BrandId { get; set; }
        public string? BrandName { get; set; }

        public string? ProductImageUrl { get; set; }
    }

    public class ProductFilterDto
    {
        public SortBy SortBy1 { get; set; } = SortBy.Popular;
        public SortBy SortBy2 { get; set; } = SortBy.LowerPrice;
        public SortBy SortBy3 { get; set; } = SortBy.StandardPrice;
        public SortBy SortBy4 { get; set; } = SortBy.MostOrdered;
    }
}
