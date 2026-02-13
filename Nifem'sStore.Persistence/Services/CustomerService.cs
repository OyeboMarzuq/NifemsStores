using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NifemsStore.Application.DTOs.CustomerDTO;
using NifemsStore.Application.DTOs.UserDTO;
using NifemsStore.Application.Interfaces.IServices;
using NifemsStores.Application.Common.Response;
using NifemsStores.Application.Interfaces.IServices;
using NifemsStores.Domain.Entities;
using NifemsStores.Persistence.Context;

namespace NifemsStores.Persistence.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICloudinaryService _cloudinaryService;

        public CustomerService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<BaseResponse<List<CustomerDto>>> GetAllCustomers()
        {
            var customers = await _context.Customers
                .Include(c => c.ApplicationUser)
                .ToListAsync();

            if (customers == null || customers.Count == 0)
            {
                return new BaseResponse<List<CustomerDto>>
                {
                    Success = false,
                    Message = "No customers found"
                };
            }

            var customerDtos = customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                ApplicationUserId = c.ApplicationUserId,
                Email = c.Email,
                Surname = c.Surname,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Avatar = c.Avatar,
                PhoneNumber = c.PhoneNumber,
                Address = c.Address,
                Gender = c.Gender,
                DateCreated = c.DateCreated
            }).ToList();

            return new BaseResponse<List<CustomerDto>>
            {
                Success = true,
                Message = "Customers retrieved successfully",
                Data = customerDtos
            };
        }

        public async Task<BaseResponse<CustomerDto>> UpdateCustomer(UpdateCustomerDto updateDto)
        {
            var customer = await _context.Customers
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(c => c.Id == updateDto.Id);

            if (customer == null)
            {
                return new BaseResponse<CustomerDto>
                {
                    Success = false,
                    Message = "Customer not found"
                };
            }

            string? avatarUrl = customer.Avatar;

            if (updateDto.Avatar != null)
            {
                avatarUrl = await _cloudinaryService.UploadImageAsync(updateDto.Avatar);
            }

            // update Customer entity
            customer.Surname = updateDto.Surname;
            customer.FirstName = updateDto.FirstName;
            customer.LastName = updateDto.LastName;
            customer.PhoneNumber = updateDto.PhoneNumber;
            customer.Address = updateDto.Address;
            customer.Gender = updateDto.Gender;
            customer.GenderDesc = updateDto.Gender.ToString();
            customer.Avatar = avatarUrl;

            var appUser = await _userManager.FindByIdAsync(customer.ApplicationUserId.ToString());

            if (appUser != null)
            {
                appUser.Surname = updateDto.Surname;
                appUser.FirstName = updateDto.FirstName;
                appUser.LastName = updateDto.LastName;
                appUser.PhoneNumber = updateDto.PhoneNumber;
                appUser.Avatar = avatarUrl;

                await _userManager.UpdateAsync(appUser);
            }

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            return new BaseResponse<CustomerDto>
            {
                Success = true,
                Message = "Customer updated successfully",
                Data = new CustomerDto
                {
                    Id = customer.Id,
                    ApplicationUserId = customer.ApplicationUserId,
                    Email = customer.Email,
                    Surname = customer.Surname,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Avatar = customer.Avatar,
                    PhoneNumber = customer.PhoneNumber,
                    Address = customer.Address,
                    Gender = customer.Gender,
                    DateCreated = customer.DateCreated
                }
            };
        }

        public async Task<BaseResponse<CustomerDto>> GetCustomerById(Guid customerId)
        {
            var customer = await _context.Customers
                .Include(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x => x.Id == customerId);

            if (customer == null)
                return BaseResponse<CustomerDto>.Failure("Customer not found", statusCode: 404);

            var customerDto = new CustomerDto
            {
                Id = customer.Id,
                ApplicationUserId = customer.ApplicationUserId,
                Email = customer.Email,
                Surname = customer.Surname,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Avatar = customer.Avatar,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                Gender = customer.Gender,
                DateCreated = customer.DateCreated
            };

            return BaseResponse<CustomerDto>.Succes(customerDto, "Customer retrieved successfully", 200);
        }

    }
}
