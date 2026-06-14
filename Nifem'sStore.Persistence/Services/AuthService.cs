using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NifemsStores.Application.DTOs;
using NifemsStores.Application.Common.Response;
using NifemsStores.Application.DTOs;
using NifemsStores.Application.Interfaces.IServices;
using NifemsStores.Domain.Entities;
using NifemsStores.Domain.Enum;
using NifemsStores.Persistence.Context;
using System.Security.Claims;

namespace NifemsStores.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly IJwtService _jwtService;
        private readonly ICloudinaryService _cloudinaryService;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, ApplicationDbContext context, IJwtService jwtService, ICloudinaryService cloudinaryService, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<BaseResponse<AdminRegistrationDto>> RegisterAdmin(AdminRegistrationDto adminDto)
        {
            var avatarUrl = adminDto.Avatar != null
                ? await _cloudinaryService.UploadImageAsync(adminDto.Avatar)
                : null;
            var user = new ApplicationUser
            {
                Avatar = avatarUrl,
                UserName = adminDto.Email,
                Email = adminDto.Email,
                Surname = adminDto.Surname,
                Gender = adminDto.Gender,
                Address = adminDto.Address,
                FirstName = adminDto.FirstName,
                LastName = adminDto.LastName,
                Nationality = adminDto.Nationality
            };

            var result = await _userManager.CreateAsync(user, adminDto.Password);
            if (!result.Succeeded)
            {
                return new BaseResponse<AdminRegistrationDto>
                {
                    Success = false,
                    Message = "Admin registration failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _userManager.AddToRoleAsync(user, Role.Admin);

                var admin = new Admin
                {
                    ApplicationUserId = user.Id,
                    Email = adminDto.Email,
                    Surname = adminDto.Surname,
                    FirstName = adminDto.FirstName,
                    LastName = adminDto.LastName,
                    Avatar = avatarUrl,
                    Gender = adminDto.Gender,
                    GenderDesc = adminDto.Gender.ToString(),
                    PhoneNumber = adminDto.PhoneNumber,
                    Address = adminDto.Address
                };

                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new BaseResponse<AdminRegistrationDto>
                {
                    Success = true,
                    Message = "Admin registered successfully",
                    Data = adminDto
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _userManager.DeleteAsync(user);

                return new BaseResponse<AdminRegistrationDto>
                {
                    Success = false,
                    Message = "Admin registration failed. Transaction rolled back.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse<UserRegistrationDto>> RegisterUser(UserRegistrationDto userDto)
        {
            if (userDto.Password != userDto.ConfirmPassword)
            {
                return new BaseResponse<UserRegistrationDto>
                {
                    Success = false,
                    Message = "Password and Confirm Password do not match"
                };
            }

            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                return new BaseResponse<UserRegistrationDto>
                {
                    Success = false,
                    Message = "User with this email already exists"
                };
            }

            var avatarUrl = userDto.Avatar != null
                ? await _cloudinaryService.UploadImageAsync(userDto.Avatar)
                : null;

            var user = new ApplicationUser
            {
                Avatar = avatarUrl,
                UserName = userDto.Email,
                Email = userDto.Email,
                Surname = userDto.Surname,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                PhoneNumber = userDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (!result.Succeeded)
            {
                return new BaseResponse<UserRegistrationDto>
                {
                    Success = false,
                    Message = "User registration failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _userManager.AddToRoleAsync(user, Role.Customer);

                var customer = new Customer
                {
                    ApplicationUserId = user.Id,
                    Email = userDto.Email,
                    Surname = userDto.Surname,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Avatar = avatarUrl,
                    Gender = userDto.Gender,
                    GenderDesc = userDto.Gender.ToString(),
                    PhoneNumber = userDto.PhoneNumber,
                    Address = userDto.Address
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new BaseResponse<UserRegistrationDto>
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _userManager.DeleteAsync(user);

                return new BaseResponse<UserRegistrationDto>
                {
                    Success = false,
                    Message = "User registration failed. Transaction rolled back.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<BaseResponse<LoginResponseDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new BaseResponse<LoginResponseDto>
                {
                    Success = false,
                    Message = "Invalid login attempt"
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user, roles.ToList());

            return new BaseResponse<LoginResponseDto>
            {
                Success = true,
                Message = "Login successful",
                Data = new LoginResponseDto
                {
                    Surname = user.Surname,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Avatar = user.Avatar,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    Token = token
                }
            };
        }

        //Google Sign Implementation
        public async Task<BaseResponse<string>> GoogleLoginAsync(IEnumerable<Claim> claims)
        {
            try
            {
                var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                var firstName = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
                var lastName = claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;

                if (string.IsNullOrWhiteSpace(email))
                    return BaseResponse<string>.Failure("Email not received from Google", statusCode: 400);

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Email = email,
                        UserName = email,
                        FirstName = firstName ?? "Google",
                        LastName = lastName ?? "User"
                    };

                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                }

                return BaseResponse<string>.Success(user.UserName, "Google login successful", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google login failed");

                return BaseResponse<string>.Failure("An error occurred during Google login", statusCode: 500);
            }
        }
    }
}
