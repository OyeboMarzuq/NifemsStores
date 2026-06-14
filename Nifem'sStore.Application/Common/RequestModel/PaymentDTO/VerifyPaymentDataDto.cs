using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.Common.RequestModel.PaymentDTO
{
    public class VerifyPaymentDataDto
    {
        public string Reference { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime? Paid_At { get; set; }
    }
}
