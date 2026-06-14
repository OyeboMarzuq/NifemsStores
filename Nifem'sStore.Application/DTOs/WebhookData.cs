using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.DTOs
{
    public class WebhookData
    {
        public string Reference { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
    }
}
