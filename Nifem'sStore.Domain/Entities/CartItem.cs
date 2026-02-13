using NifemsStore.Domain.Entities;

namespace NifemsStores.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public Guid CartItemId { get; set; }
        public Cart Cart { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public Guid CartId { get; set; }
    }
}
