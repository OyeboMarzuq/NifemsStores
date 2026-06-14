using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.DTOs
{
    public class PaystackWebhookDto
    {
        public string Event { get; set; }
        public WebhookData Data { get; set; }
    }
}
