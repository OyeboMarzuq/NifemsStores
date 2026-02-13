using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NifemsStore.Application.DTOs;
using NifemsStore.Application.DTOs.CustomerDTO;
using NifemsStore.Application.DTOs.UserDTO;
using NifemsStore.Application.Interfaces.IServices;
using NifemsStores.Application.Common.Response;
using NifemsStores.Application.Interfaces.IServices;

namespace NifemsStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICustomerService _customerService;

        public UserController(IAuthService authService, ICustomerService customerService)
        {
            _authService = authService;
            _customerService = customerService;
        }


        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromForm] UserRegistrationDto dto)
        {
            var response = await _authService.RegisterUser(dto);

            if (!response.Success)
                return StatusCode(400, response);

            return StatusCode(201, response);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var response = await _customerService.GetAllCustomers();

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCustomer([FromForm] UpdateCustomerDto updateDto)
        {
            if (updateDto == null)
                return BadRequest("Customer data is required"); 

            var response = await _customerService.UpdateCustomer(updateDto);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpGet("get-customer/{id}")]
        [ProducesResponseType(typeof(BaseResponse<CustomerDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<CustomerDto>), 404)]
        public async Task<IActionResult> GetCustomerById([FromRoute] Guid id)
        {
            var response = await _customerService.GetCustomerById(id);

            return StatusCode(response.StatusCode ?? 200, response);
        }
    }
}
