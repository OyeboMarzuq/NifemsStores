using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Application.DTOs.CartDTO
{
    public class CartDto
    {
        public Guid UserId { get; set; }
        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
        public decimal TotalAmount { get; set; }
        public Guid CartId { get; set; }
    }
}
