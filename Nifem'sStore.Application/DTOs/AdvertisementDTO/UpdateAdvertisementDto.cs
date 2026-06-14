using NifemsStore.Domain.Enum;

namespace NifemsStores.Application.DTOs.AdvertisementDTO
{
    public class UpdateAdvertisementDto
    {
        public Guid Id { get; set; }
        public string AdvertImg { get; set; } = default!;
        public string AdvertDesc { get; set; } = default!;
        public AdsPlatform AdsPlatform { get; set; }
    }
}
