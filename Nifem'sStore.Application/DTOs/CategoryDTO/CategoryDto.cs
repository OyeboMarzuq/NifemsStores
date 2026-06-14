namespace NifemsStores.Application.DTOs.CategoryDTO
{
    public class CategoryDto
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
