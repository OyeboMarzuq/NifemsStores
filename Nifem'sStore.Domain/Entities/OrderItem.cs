
using NifemsStores.Domain.Entities;
using NifemsStores.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid(); 
    public Guid OrderId { get; set; }         
    public Order Order { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal => Quantity * UnitPrice;
}
