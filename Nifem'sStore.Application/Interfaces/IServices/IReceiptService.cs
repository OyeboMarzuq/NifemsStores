using NifemsStore.Application.DTOs.RecieptDTO;
using NifemsStores.Application.Common.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStore.Application.Interfaces.IServices
{
    public interface IReceiptService
    {
        Task<BaseResponse<ReceiptDto>> CreateReceipt(CreateReceiptDto dto, Guid userId, string userName);
        Task<BaseResponse<List<ReceiptDto>>> GetMyReceipts(Guid userId);
        Task<BaseResponse<ReceiptDto>> GetReceiptById(Guid receiptId, Guid userId, bool isAdmin);
    }
}
