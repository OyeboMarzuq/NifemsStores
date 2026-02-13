using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Domain.Enum
{
    public enum OrderStatus
    {
        Pending = 1,
        Processing,
        Delivered,
        Shipped,
        Cancelled,
        Returned
    }
}
