using NifemsStores.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Domain.Entities
{
    public class Category : BaseEntity
    {
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public Guid VendorId { get; set; }
        //The Product Collection Is Created Because Product Will Be Found Inside Category
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
