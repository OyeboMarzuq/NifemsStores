using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NifemsStore.Application.DTOs.CategoryDTO;
using NifemsStore.Application.Interfaces.IServices;
using NifemsStore.Domain.Entities;
using NifemsStores.Application.Common.Response;
using NifemsStores.Domain.Entities;
using NifemsStores.Persistence.Context;

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

    public async Task<BaseResponse<CategoryDto>> CreateCategory(CreateCategoryRequestDto dto, Guid vendorId, string performedBy)
    {
        _logger.LogInformation("Vendor {VendorId} is creating a category", vendorId);

        var existingCategory = await _context.Categories
            .FirstOrDefaultAsync(x => x.Name.ToLower() == dto.Name.ToLower() && x.VendorId == vendorId);

        if (existingCategory != null)
        {
            _logger.LogWarning("Category {CategoryName} already exists for Vendor {VendorId}", dto.Name, vendorId);
            return BaseResponse<CategoryDto>.Failure("Category already exists", statusCode: 409);
        }

        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            VendorId = vendorId,
            UpdatedAt = DateTime.Now
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(new AuditLog
        {
            Action = "CREATE",
            EntityName = "Category",
            EntityId = category.CategoryId.ToString(),
            PerformedBy = performedBy,
        });

        _logger.LogInformation("Category created successfully: {CategoryId}", category.CategoryId);

        return BaseResponse<CategoryDto>.Succes(new CategoryDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            Description = category.Description,
            VendorId = category.VendorId,
            UpdatedAt = category.UpdatedAt
        }, "Category created successfully", 201);
    }

    public async Task<BaseResponse<List<CategoryDto>>> GetAllCategories(Guid vendorId)
    {
        _logger.LogInformation("Fetching categories for Vendor {VendorId}", vendorId);

        var categories = await _context.Categories
            .Where(x => x.VendorId == vendorId)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync();

        var response = categories.Select(x => new CategoryDto
        {
            CategoryId = x.CategoryId,
            Name = x.Name,
            Description = x.Description,
            VendorId = x.VendorId,
            UpdatedAt = x.UpdatedAt
        }).ToList();

        return BaseResponse<List<CategoryDto>>.Succes(response, "Categories retrieved successfully", 200);
    }

    public async Task<BaseResponse<CategoryDto>> GetCategoryById(Guid categoryId, Guid vendorId)
    {
        _logger.LogInformation("Fetching category {CategoryId} for Vendor {VendorId}", categoryId, vendorId);

        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.CategoryId == categoryId && x.VendorId == vendorId);

        if (category == null)
        {
            _logger.LogWarning("Category not found: {CategoryId}", categoryId);
            return BaseResponse<CategoryDto>.Failure("Category not found", statusCode: 404);
        }

        return BaseResponse<CategoryDto>.Succes(new CategoryDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            Description = category.Description,
            VendorId = category.VendorId,
            UpdatedAt = category.UpdatedAt
        }, "Category retrieved successfully", 200);
    }

    public async Task<BaseResponse<CategoryDto>> UpdateCategory(Guid categoryId, UpdateCategoryDto dto, Guid vendorId, string performedBy)
    {
        _logger.LogInformation("Vendor {VendorId} is updating category {CategoryId}", vendorId, categoryId);

        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.CategoryId == categoryId && x.VendorId == vendorId);

        if (category == null)
        {
            _logger.LogWarning("Category not found for update: {CategoryId}", categoryId);
            return BaseResponse<CategoryDto>.Failure("Category not found", statusCode: 404);
        }

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.UpdatedAt = DateTime.Now;

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(new AuditLog
        {
            Action = "UPDATE",
            EntityName = "Category",
            EntityId = category.CategoryId.ToString(),
            PerformedBy = performedBy,
        });

        _logger.LogInformation("Category updated successfully: {CategoryId}", category.CategoryId);

        return BaseResponse<CategoryDto>.Succes(new CategoryDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            Description = category.Description,
            VendorId = category.VendorId,
            UpdatedAt = category.UpdatedAt
        }, "Category updated successfully", 200);
    }

    public async Task<BaseResponse<string>> DeleteCategory(Guid categoryId, Guid vendorId, string performedBy)
    {
        _logger.LogInformation("Vendor {VendorId} is deleting category {CategoryId}", vendorId, categoryId);

        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.CategoryId == categoryId && x.VendorId == vendorId);

        if (category == null)
        {
            _logger.LogWarning("Category not found for deletion: {CategoryId}", categoryId);
            return BaseResponse<string>.Failure("Category not found", statusCode: 404);
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(new AuditLog
        {
            Action = "DELETE",
            EntityName = "Category",
            EntityId = category.CategoryId.ToString(),
            PerformedBy = performedBy,
        });

        _logger.LogInformation("Category deleted successfully: {CategoryId}", categoryId);

        return BaseResponse<string>.Succes("Deleted", "Category deleted successfully", 200);
    }
}
