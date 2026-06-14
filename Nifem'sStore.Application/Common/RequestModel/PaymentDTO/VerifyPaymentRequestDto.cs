using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Application.Common.RequestModel.PaymentDTO
{
    public class VerifyPaymentRequestDto
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public VerifyPaymentDataDto Data { get; set; }
        public DateTime DateRequested { get; set; }
    }
}
