using NifemsStores.Domain.Enum;

namespace NifemsStores.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Surname { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        public string? Avatar { get; set; }

        public string PhoneNumber { get; set; } = default!;
        public string Address { get; set; } = default!;

        public Gender Gender { get; set; }
        public string GenderDesc { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Navigation collections (E-commerce normal features)
        //public ICollection<Order> Orders { get; set; } = new List<Order>();
        //public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}
