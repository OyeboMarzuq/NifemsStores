namespace NifemsStores.Domain.Entities
{
    public class PaystackSplit
    {
        public int Id { get; set; }
        public string SplitCode { get; set; } = default!;
        public string SplitName { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
