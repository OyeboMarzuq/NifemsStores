using NifemsStore.Application.DTOs.CustomerDTO;
using NifemsStores.Application.DTOs.UserDTO;
using NifemsStores.Application.Common.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Application.Interfaces.IServices
{
    public interface ICustomerService
    {
        Task<BaseResponse<List<CustomerDto>>> GetAllCustomers();
        Task<BaseResponse<CustomerDto>> UpdateCustomer(UpdateCustomerDto updateDto);
        Task<BaseResponse<CustomerDto>> GetCustomerById(Guid customerId);
    }
}
