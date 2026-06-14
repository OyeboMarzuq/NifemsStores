using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NifemsStores.Application.DTOs.CategoryDTO;
using NifemsStores.Application.Interfaces.IServices;
using NifemsStores.Application.Common.Response;
using NifemsStores.Domain.Entities;
using NifemsStores.Persistence.Context;

namespace NifemsStores.Persistence.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryService> _logger;
        private readonly IAuditLogService _auditLogService;

        public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger, IAuditLogService auditLogService)
        {
            _context = context;
            _logger = logger;
            _auditLogService = auditLogService;
        }

        public async Task<BaseResponse<CategoryDto>> CreateCategory(CreateCategoryRequestDto dto, string performedBy)
        {
            _logger.LogInformation("Admin creating category {Name}", dto.Name);

            var existing = await _context.Categories
                .FirstOrDefaultAsync(x => x.Name.ToLower() == dto.Name.ToLower());

            if (existing != null)
                return BaseResponse<CategoryDto>.Failure("Category already exists", statusCode: 409);

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(new AuditLog
            {
                Action = "CREATE",
                EntityName = "Category",
                EntityId = category.CategoryId.ToString(),
                PerformedBy = performedBy,
                Role = "Admin"
            });

            return BaseResponse<CategoryDto>.Success(new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                UpdatedAt = category.UpdatedAt
            }, "Category created successfully", 201);
        }

        public async Task<BaseResponse<List<CategoryDto>>> GetAllCategories()
        {
            var categories = await _context.Categories
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();

            var response = categories.Select(x => new CategoryDto
            {
                CategoryId = x.CategoryId,
                Name = x.Name,
                Description = x.Description,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return BaseResponse<List<CategoryDto>>.Success(response, "Categories retrieved successfully", 200);
        }

        public async Task<BaseResponse<CategoryDto>> GetCategoryById(Guid categoryId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == categoryId);

            if (category == null)
                return BaseResponse<CategoryDto>.Failure("Category not found", statusCode: 404);

            return BaseResponse<CategoryDto>.Success(new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                UpdatedAt = category.UpdatedAt
            }, "Category retrieved successfully", 200);
        }

        public async Task<BaseResponse<CategoryDto>> UpdateCategory(Guid categoryId, UpdateCategoryDto dto, string performedBy)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == categoryId);

            if (category == null)
                return BaseResponse<CategoryDto>.Failure("Category not found", statusCode: 404);

            category.Name = dto.Name;
            category.Description = dto.Description;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(new AuditLog
            {
                Action = "UPDATE",
                EntityName = "Category",
                EntityId = category.CategoryId.ToString(),
                PerformedBy = performedBy,
                Role = "Admin"
            });

            return BaseResponse<CategoryDto>.Success(new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                UpdatedAt = category.UpdatedAt
            }, "Category updated successfully", 200);
        }

        public async Task<BaseResponse<string>> DeleteCategory(Guid categoryId, string performedBy)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == categoryId);

            if (category == null)
                return BaseResponse<string>.Failure("Category not found", statusCode: 404);

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(new AuditLog
            {
                Action = "DELETE",
                EntityName = "Category",
                EntityId = category.CategoryId.ToString(),
                PerformedBy = performedBy,
                Role = "Admin"
            });

            return BaseResponse<string>.Success("Deleted", "Category deleted successfully", 200);
        }
    }
}
