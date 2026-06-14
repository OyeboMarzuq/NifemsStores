using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Application.Common.Response
{
    public class PaymentVerificationResponseDto
    {
        public string Reference { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string GatewayResponse { get; set; }
        public DateTime VerificationDate { get; set; }
    }
}
