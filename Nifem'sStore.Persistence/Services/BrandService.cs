using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NifemsStores.Application.DTOs.BrandDTO;
using NifemsStores.Application.Interfaces.IServices;
using NifemsStores.Domain.Entities;
using NifemsStores.Application.Common.Response;
using NifemsStores.Persistence.Context;

namespace NifemsStores.Persistence.Services
{
    public class BrandService : IBrandService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BrandService> _logger;

        public BrandService(ApplicationDbContext context, ILogger<BrandService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BaseResponse<BrandDto>> CreateBrand(CreateBrandRequestDto request)
        {
            try
            {
                _logger.LogInformation("Creating a new Brand with Name: {BrandName}", request.Name);

                var existingBrand = await _context.Brands
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == request.Name.ToLower());

                if (existingBrand != null)
                {
                    _logger.LogWarning("Brand creation failed. Brand already exists: {BrandName}", request.Name);
                    return BaseResponse<BrandDto>.Failure("Brand already exists", statusCode: 409);
                }

                var brand = new Brand
                {
                    Name = request.Name,
                    Description = request.Description
                };

                await _context.Brands.AddAsync(brand);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Brand created successfully with Id: {BrandId}", brand.Id);

                var brandDto = new BrandDto
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    Description = brand.Description,
                };

                return BaseResponse<BrandDto>.Success(brandDto, "Brand created successfully", 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating Brand with Name: {BrandName}", request.Name);

                return BaseResponse<BrandDto>.Failure("An error occurred while creating brand", statusCode: 500);
            }
        }

        public async Task<BaseResponse<List<BrandDto>>> GetAllBrands()
        {
            try
            {
                _logger.LogInformation("Fetching all brands...");

                var brands = await _context.Brands.ToListAsync();

                _logger.LogInformation("Fetched {Count} brands successfully", brands.Count);

                var brandDtos = brands.Select(b => new BrandDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                }).ToList();

                return BaseResponse<List<BrandDto>>.Success(brandDtos, "Brands retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all brands");

                return BaseResponse<List<BrandDto>>.Failure("An error occurred while retrieving brands", statusCode: 500);
            }
        }

        public async Task<BaseResponse<BrandDto>> GetBrandById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching brand with Id: {BrandId}", id);

                var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == id);

                if (brand == null)
                {
                    _logger.LogWarning("Brand not found with Id: {BrandId}", id);
                    return BaseResponse<BrandDto>.Failure("Brand not found", statusCode: 404);
                }

                var brandDto = new BrandDto
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    Description = brand.Description,
                };

                _logger.LogInformation("Brand retrieved successfully with Id: {BrandId}", id);

                return BaseResponse<BrandDto>.Success(brandDto, "Brand retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving brand with Id: {BrandId}", id);

                return BaseResponse<BrandDto>.Failure("An error occurred while retrieving brand", statusCode: 500);
            }
        }

        public async Task<BaseResponse<BrandDto>> UpdateBrand(UpdateBrandDto request)
        {
            try
            {
                _logger.LogInformation("Updating brand with Id: {BrandId}", request.Id);

                var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == request.Id);

                if (brand == null)
                {
                    _logger.LogWarning("Update failed. Brand not found with Id: {BrandId}", request.Id);
                    return BaseResponse<BrandDto>.Failure("Brand not found", statusCode: 404);
                }

                brand.Name = request.Name;
                brand.Description = request.Description;

                _context.Brands.Update(brand);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Brand updated successfully with Id: {BrandId}", brand.Id);

                var brandDto = new BrandDto
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    Description = brand.Description,
                };

                return BaseResponse<BrandDto>.Success(brandDto, "Brand updated successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating brand with Id: {BrandId}", request.Id);

                return BaseResponse<BrandDto>.Failure("An error occurred while updating brand", statusCode: 500);
            }
        }

        public async Task<BaseResponse<bool>> DeleteBrand(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting brand with Id: {BrandId}", id);

                var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == id);

                if (brand == null)
                {
                    _logger.LogWarning("Delete failed. Brand not found with Id: {BrandId}", id);
                    return BaseResponse<bool>.Failure("Brand not found", statusCode: 404);
                }

                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Brand deleted successfully with Id: {BrandId}", id);

                return BaseResponse<bool>.Success(true, "Brand deleted successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting brand with Id: {BrandId}", id);

                return BaseResponse<bool>.Failure("An error occurred while deleting brand", statusCode: 500);
            }
        }
    }
}
