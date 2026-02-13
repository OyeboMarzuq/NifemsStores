using NifemsStore.Application.DTOs;
using NifemsStores.Application.Common.Response;
using NifemsStores.Application.DTOs;
using System.Security.Claims;

namespace NifemsStores.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<BaseResponse<AdminRegistrationDto>> RegisterAdmin(AdminRegistrationDto adminDto);   
        Task<BaseResponse<UserRegistrationDto>> RegisterUser(UserRegistrationDto userDto);
        Task<BaseResponse<LoginResponseDto>> Login(LoginDto loginDto);
        Task<BaseResponse<string>> GoogleLoginAsync(IEnumerable<Claim> claims);
    }
}
