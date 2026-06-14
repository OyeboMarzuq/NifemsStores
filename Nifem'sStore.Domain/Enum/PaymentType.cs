using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Domain.Enum
{
    public enum PaymentType
    {
        Cash = 1,
        BankTransfer,
        Cheque,
        CreditCard,
        None,
        CashOnDelivery
    }
}
