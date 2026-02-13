using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NifemsStore.Application.DTOs.AdvertisementDTO;
using NifemsStore.Application.Interfaces.IServices;
using NifemsStores.Application.Common.Response;
using NifemsStores.Domain.Entities;
using NifemsStores.Persistence.Context;
using System.Text.Json;

namespace NifemsStores.Persistence.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdvertisementService> _logger;
        private readonly IAuditLogService _auditLogService;

        public AdvertisementService(ApplicationDbContext context, ILogger<AdvertisementService> logger, IAuditLogService auditLogService)
        {
            _context = context;
            _logger = logger;
            _auditLogService = auditLogService;
        }

        public async Task<BaseResponse<AdvertisementDto>> CreateAsync(CreateAdvertisementDto dto, string performedBy)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.AdvertImg))
                    return BaseResponse<AdvertisementDto>.Failure("Advert image is required", statusCode: 400);

                if (string.IsNullOrWhiteSpace(dto.AdvertDesc))
                    return BaseResponse<AdvertisementDto>.Failure("Advert description is required", statusCode: 400);

                var advert = new Advertisement
                {
                    Id = Guid.NewGuid(),
                    AdvertImg = dto.AdvertImg.Trim(),
                    AdvertDesc = dto.AdvertDesc.Trim(),
                    AdsPlatform = dto.AdsPlatform,
                };

                await _context.Advertisements.AddAsync(advert);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(new AuditLog
                {
                    Action = "CREATE",
                    EntityName = "Advertisement",
                    EntityId = advert.Id.ToString(),
                    NewValues = JsonSerializer.Serialize(advert),
                    PerformedBy = performedBy,
                    Role = "Admin"
                });

                _logger.LogInformation("Advertisement created successfully: {AdvertId}", advert.Id);

                var response = new AdvertisementDto
                {
                    Id = advert.Id,
                    AdvertImg = advert.AdvertImg,
                    AdvertDesc = advert.AdvertDesc,
                    AdsPlatform = advert.AdsPlatform,
                };

                return BaseResponse<AdvertisementDto>.Succes(response, "Advertisement created successfully", 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating advertisement");
                return BaseResponse<AdvertisementDto>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<AdvertisementDto>> UpdateAsync(UpdateAdvertisementDto dto, string performedBy)
        {
            try
            {
                var advert = await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == dto.Id);

                if (advert == null)
                    return BaseResponse<AdvertisementDto>.Failure("Advertisement not found", statusCode: 404);

                var oldValues = JsonSerializer.Serialize(advert);

                advert.AdvertImg = dto.AdvertImg.Trim();
                advert.AdvertDesc = dto.AdvertDesc.Trim();
                advert.AdsPlatform = dto.AdsPlatform;

                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(new AuditLog
                {
                    Action = "UPDATE",
                    EntityName = "Advertisement",
                    EntityId = advert.Id.ToString(),
                    OldValues = oldValues,
                    NewValues = JsonSerializer.Serialize(advert),
                    PerformedBy = performedBy,
                    Role = "Admin"
                });

                _logger.LogInformation("Advertisement updated successfully: {AdvertId}", advert.Id);

                var response = new AdvertisementDto
                {
                    Id = advert.Id,
                    AdvertImg = advert.AdvertImg,
                    AdvertDesc = advert.AdvertDesc,
                    AdsPlatform = advert.AdsPlatform,
                };

                return BaseResponse<AdvertisementDto>.Succes(response, "Advertisement updated successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating advertisement");
                return BaseResponse<AdvertisementDto>.Failure("Something went wrong", statusCode: 500);
            }
        }

        public async Task<BaseResponse<string>> DeleteAsync(Guid id, string performedBy)
        {
            try
            {
                var advert = await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == id);

                if (advert == null)
                    return BaseResponse<string>.Failure("Advertisement not found", statusCode: 404);

                var oldValues = JsonSerializer.Serialize(advert);

                _context.Advertisements.Remove(advert);
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(new AuditLog
                {
                    Action = "DELETE",
                    EntityName = "Advertisement",
                    EntityId = advert.Id.ToString(),
                    OldValues = oldValues,
                    PerformedBy = performedBy,
                    Role = "Admin"
                });

                _logger.LogWarning("Advertisement deleted: {AdvertId}", advert.Id);

                return BaseResponse<string>.Succes("Deleted", "Advertisement deleted successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting advertisement");
                return BaseResponse<string>.Failure("Something went wrong", statusCode: 500);
            }
        }
    }
}
