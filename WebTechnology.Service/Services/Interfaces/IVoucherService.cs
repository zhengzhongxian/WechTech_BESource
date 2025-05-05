
using Microsoft.AspNetCore.JsonPatch;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Vouchers;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IVoucherService
    {
        Task<ServiceResponse<Voucher>> GetVoucherAsync(string idVoucher);
        Task<ServiceResponse<Voucher>> CreateVoucherAsync(CreateVoucherDTO createDto);
        Task<ServiceResponse<Voucher>> UpdateVoucherAsync(string idVoucher, JsonPatchDocument<Voucher> patchDoc);
        Task<ServiceResponse<bool>> DeleteVoucherAsync(string idVoucher);
    }
}
