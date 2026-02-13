
using NifemsStore.Domain.Enum;
using NifemsStores.Domain.Entities;

public class Order : BaseEntity
{
    // This Shows On The Vendors Dashboard
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; }
    public PaymentType PaymentType { get; set; } = PaymentType.None;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public Guid VendorId { get; set; }

    public Guid GuestUserId { get; set; }
    public string GuestUser { get; set; }
}
