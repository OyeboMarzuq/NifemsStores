using Microsoft.AspNetCore.Http;

namespace NifemsStores.Application.Interfaces.IServices
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
