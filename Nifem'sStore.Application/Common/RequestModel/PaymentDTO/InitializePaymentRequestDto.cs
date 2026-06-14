using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.Common.RequestModel.PaymentDTO
{
    public class InitializePaymentRequestDto
    {
        public decimal Amount { get; set; }
        public string Email { get; set; }
        public string? Reference { get; set; }
    }
}
