using NifemsStore.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.DTOs.AdvertisementDTO
{
    public class CreateAdvertisementDto
    {
        public string AdvertImg { get; set; } = default!;
        public string AdvertDesc { get; set; } = default!;
        public AdsPlatform AdsPlatform { get; set; }
    }
}
