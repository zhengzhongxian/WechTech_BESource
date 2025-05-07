using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface IApplyVoucherRepository : IGenericRepository<ApplyVoucher>
    {
        Task<IEnumerable<ApplyVoucher>> GetByOrderIdAsync(string orderId);
        Task<bool> ApplyVoucherToOrderAsync(string orderId, string voucherId);
        Task<bool> RemoveVoucherFromOrderAsync(string orderId, string voucherId);
    }
}
