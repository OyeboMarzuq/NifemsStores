using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NifemsStores.Application.DTOs;
using NifemsStores.Application.Common.Response;
using NifemsStores.Application.DTOs;
using NifemsStores.Application.Interfaces.IServices;

namespace NifemsStores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<AdminRegistrationDto> _adminDtoValidator;
        private readonly IValidator<LoginDto> _loginDtoValidator;

        public AuthController(
            IAuthService authService,
            IValidator<AdminRegistrationDto> adminDtoValidator,
            IValidator<LoginDto> loginDtoValidator)
        {
            _authService = authService;
            _adminDtoValidator = adminDtoValidator;
            _loginDtoValidator = loginDtoValidator;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromForm] AdminRegistrationDto adminDto)
        {
            var validationResult = await _adminDtoValidator.ValidateAsync(adminDto);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                {
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            var response = await _authService.RegisterAdmin(adminDto);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromForm] UserRegistrationDto dto)
        {
            var response = await _authService.RegisterUser(dto);

            if (!response.Success)
                return StatusCode(400, response);

            return StatusCode(201, response);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var validationResult = await _loginDtoValidator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                {
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            var response = await _authService.Login(loginDto);
            if (!response.Success)
                return Unauthorized(response);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("google-signin")]
        public IActionResult GoogleSignIn()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleResponse))
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet("google-response")]
        [ProducesResponseType(typeof(BaseResponse<string>), 200)]
        [ProducesResponseType(typeof(BaseResponse<string>), 401)]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                var response = BaseResponse<string>.Unauthorized("Google authentication failed");
                return StatusCode(response.StatusCode.Value, response);
            }

            var claims = result.Principal?.Claims;

            if (claims == null)
            {
                var response = BaseResponse<string>.Unauthorized("No claims received from Google");
                return StatusCode(response.StatusCode.Value, response);
            }

            var responseData = await _authService.GoogleLoginAsync(claims);

            return StatusCode(responseData.StatusCode ?? 200, responseData);
        }
    }
}