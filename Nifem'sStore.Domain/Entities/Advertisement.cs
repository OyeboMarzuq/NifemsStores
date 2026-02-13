
using NifemsStore.Domain.Enum;

namespace NifemsStores.Domain.Entities
{
    public class Advertisement : BaseEntity
    {
        public string AdvertImg { get; set; }
        public string AdvertDesc { get; set; }
        public AdsPlatform AdsPlatform { get; set; }
    }
}
