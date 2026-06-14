using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NifemsStore.Application.DTOs;
using NifemsStore.Application.DTOs.ProductDTO;
using NifemsStore.Application.Helper;
using NifemsStore.Application.Interfaces.IServices;
using NifemsStore.Domain.Entities;
using NifemsStores.Application.Common.Response;
using NifemsStores.Application.DTOs;
using NifemsStores.Application.Interfaces.IServices;
using NifemsStores.Domain.Entities;
using NifemsStores.Persistence.Context;
using System.Text.Json;

namespace NifemsStores.Persistence.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;
        private readonly IAuditLogService _auditLogService;
        private readonly IMemoryCache _cache;

        public ProductService(ApplicationDbContext context, ILogger<ProductService> logger, IAuditLogService auditLogService, IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _auditLogService = auditLogService;
            _cache = cache;
        }

        public async Task<BaseResponse<CreateProductRequestDto>> CreateProduct(CreateProductRequestDto dto, Guid vendorId, string userName)
        {
            try
            {
                _logger.LogInformation("Creating product {Name} by vendor {VendorId}", dto.Name, vendorId);

                var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == dto.CategoryId);
                if (category == null)
                    return BaseResponse<CreateProductRequestDto>.Failure("Category not found", statusCode: 404);

                var packSellingPrice = Calculator.CalculatePackSellingPrice(dto.PricePerPack, dto.PackPriceMarkup);
                var unitSellingPrice = Calculator.CalculateUnitSellingPrice(packSellingPrice, dto.TotalItemInPack);

                var product = new Product
                {
                    ProductId = Guid.NewGuid(),
                    Name = dto.Name,
                    Description = dto.Description,
                    UnitPrice = unitSellingPrice,
                    CategoryId = dto.CategoryId,
                    BrandId = dto.BrandId,
                    ProductImageUrl = dto.ProductImageUrl,
                    QuantityInStock = dto.QuantityInStock,
                    Vendor = vendorId,
                    UserName = userName,
                };

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(new AuditLog
                {
                    Action = "CREATE",
                    EntityName = "Product",
                    EntityId = product.ProductId.ToString(),
                    NewValues = JsonSerializer.Serialize(product),
                    PerformedBy = userName,
                    Role = "Vendor/Admin"
                });

                _logger.LogInformation("Product created successfully: {ProductId}", product.ProductId);

                return BaseResponse<CreateProductRequestDto>.Succes(dto, "Product created successfully", 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product");
                return BaseResponse<CreateProductRequestDto>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<UpdateProductDto>> UpdateProduct(UpdateProductDto dto, Guid vendorId, bool isAdmin)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == dto.ProductId);

                if (product == null)
                    return BaseResponse<UpdateProductDto>.Failure("Product not found", statusCode: 404);

                if (!isAdmin && product.Vendor != vendorId)
                    return BaseResponse<UpdateProductDto>.Failure("You are not authorized to update this product", statusCode: 403);

                var oldValues = JsonSerializer.Serialize(product);

                var packSellingPrice = Calculator.CalculatePackSellingPrice(dto.PricePerPack, dto.PackPriceMarkup);
                var unitSellingPrice = Calculator.CalculateUnitSellingPrice(packSellingPrice, dto.TotalItemInPack);

                product.Name = dto.Name;
                product.Description = dto.Description;
                product.PricePerPack = dto.PricePerPack;
                product.PackPriceMarkup = dto.PackPriceMarkup;
                product.UnitPrice = unitSellingPrice;
                product.TotalItemInPack = dto.TotalItemInPack;
                product.CategoryId = dto.CategoryId;
                product.BrandId = dto.BrandId;
                product.ProductImageUrl = dto.ProductImageUrl;
                product.QuantityInStock = dto.QuantityInStock;

                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(new AuditLog
                {
                    Action = "UPDATE",
                    EntityName = "Product",
                    EntityId = product.ProductId.ToString(),
                    OldValues = oldValues,
                    NewValues = JsonSerializer.Serialize(product),
                    PerformedBy = vendorId.ToString(),
                    Role = isAdmin ? "Admin" : "Vendor"
                });

                _logger.LogInformation("Product updated successfully: {ProductId}", product.ProductId);

                return BaseResponse<UpdateProductDto>.Succes(dto, "Product updated successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product");
                return BaseResponse<UpdateProductDto>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<string>> DeleteProduct(Guid productId, Guid vendorId, bool isAdmin)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId);

                if (product == null)
                    return BaseResponse<string>.Failure("Product not found", statusCode: 404);

                if (!isAdmin && product.Vendor != vendorId)
                    return BaseResponse<string>.Failure("You are not authorized to delete this product", statusCode: 403);

                var oldValues = JsonSerializer.Serialize(product);

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(new AuditLog
                {
                    Action = "DELETE",
                    EntityName = "Product",
                    EntityId = product.ProductId.ToString(),
                    OldValues = oldValues,
                    PerformedBy = vendorId.ToString(),
                    Role = isAdmin ? "Admin" : "Vendor"
                });

                _logger.LogWarning("Product deleted: {ProductId}", productId);

                return BaseResponse<string>.Succes("Product deleted successfully", "Deleted", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product");
                return BaseResponse<string>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<PaginatedResponse<ProductDto>>> GetAllProducts(ProductFilterRequestDto filter, Guid vendorId, bool isAdmin)
        {
            try
            {
                // ✅ Cache Key (unique per vendor/admin and filters)
                string cacheKey = $"products_{vendorId}_{isAdmin}_{filter.PageNumber}_{filter.PageSize}_{filter.CategoryId}_{filter.BrandId}_{filter.Search}";

                // ✅ Check Cache
                if (_cache.TryGetValue(cacheKey, out PaginatedResponse<ProductDto> cachedResponse))
                {
                    _logger.LogInformation("Products retrieved from cache: {CacheKey}", cacheKey);

                    return BaseResponse<PaginatedResponse<ProductDto>>
                        .Succes(cachedResponse, "Products retrieved successfully (cached)", 200);
                }

                var query = _context.Products
                    .Include(x => x.Category)
                    .Include(x => x.Brand)
                    .AsQueryable();

                // 🔥 Vendor should only see his products
                if (!isAdmin)
                {
                    query = query.Where(x => x.Vendor == vendorId);
                }

                // Filter by category
                if (filter.CategoryId.HasValue)
                {
                    query = query.Where(x => x.CategoryId == filter.CategoryId.Value);
                }

                // Filter by brand
                if (filter.BrandId.HasValue)
                {
                    query = query.Where(x => x.BrandId == filter.BrandId.Value);
                }

                // Search by name
                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    query = query.Where(x => x.Name.Contains(filter.Search));
                }

                var totalRecords = await query.CountAsync();

                var products = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(x => new ProductDto
                    {
                        ProductId = x.ProductId,
                        Name = x.Name,
                        Description = x.Description,
                        PricePerPack = x.PricePerPack,
                        PackPriceMarkup = x.PackPriceMarkup,
                        UnitPrice = x.UnitPrice,
                        TotalItemInPack = x.TotalItemInPack,
                        QuantityInStock = x.QuantityInStock,
                        CategoryId = x.CategoryId,
                        CategoryName = x.Category.Name,
                        BrandId = x.BrandId,
                        ProductImageUrl = x.ProductImageUrl,
                        BrandName = x.Brand != null ? x.Brand.Name : null,
                        VendorId = x.Vendor,
                        VendorUserName = x.UserName
                    })
                    .ToListAsync();

                var response = new PaginatedResponse<ProductDto>
                {
                    Items = products,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalRecords = totalRecords,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
                };

                // ✅ Save to Cache
                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(3));

                _logger.LogInformation("Products cached successfully: {CacheKey}", cacheKey);

                return BaseResponse<PaginatedResponse<ProductDto>>
                    .Succes(response, "Products retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products");
                return BaseResponse<PaginatedResponse<ProductDto>>
                    .Failure("Something went wrong", statusCode: 500);
            }
        }



        public async Task<BaseResponse<ProductDto>> GetProductById(Guid productId)
        {
            try
            {
                string cacheKey = $"product_{productId}";

                // ✅ Check cache
                if (_cache.TryGetValue(cacheKey, out ProductDto cachedProduct))
                {
                    _logger.LogInformation("Product retrieved from cache: {ProductId}", productId);

                    return BaseResponse<ProductDto>.Succes(cachedProduct, "Product retrieved successfully (cached)", 200);
                }

                var product = await _context.Products
                    .Include(x => x.Category)
                    .Include(x => x.Brand)
                    .FirstOrDefaultAsync(x => x.ProductId == productId);

                if (product == null)
                    return BaseResponse<ProductDto>.Failure("Product not found", statusCode: 404);

                var response = new ProductDto
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    PricePerPack = product.PricePerPack,
                    PackPriceMarkup = product.PackPriceMarkup,
                    UnitPrice = product.UnitPrice,
                    TotalItemInPack = product.TotalItemInPack,
                    QuantityInStock = product.QuantityInStock,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name,
                    BrandId = product.BrandId,
                    BrandName = product.Brand != null ? product.Brand.Name : null,
                    ProductImageUrl = product.ProductImageUrl,
                    VendorId = product.Vendor,
                    VendorUserName = product.UserName
                };

                // ✅ Save to cache
                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));

                _logger.LogInformation("Product cached successfully: {ProductId}", productId);

                return BaseResponse<ProductDto>.Succes(response, "Product retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching product");
                return BaseResponse<ProductDto>.Failure("Something went wrong", statusCode: 500);
            }
        }

    }
}
