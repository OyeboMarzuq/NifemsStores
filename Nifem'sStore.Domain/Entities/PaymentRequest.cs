using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Domain.Entities
{
    public class PaymentRequest
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string TransactionReference { get; set; }
        public string Status { get; set; }
        public DateTime? DatePaid { get; set; }
        public DateTime DateRequested { get; set; } = DateTime.Now;
    }
}
