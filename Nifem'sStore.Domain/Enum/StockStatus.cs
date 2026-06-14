using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Domain.Enum
{
    public enum StockStatus
    {
        Available = 1,
        LowStock,
        PendingApproval,
        OutOfStock
    }
}
