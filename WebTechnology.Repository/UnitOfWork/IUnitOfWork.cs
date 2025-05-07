using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IImageRepository Images { get; }
        IVoucherRepository Vouchers { get; }
        IApplyVoucherRepository ApplyVouchers { get; }

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> SaveChangesAsync();
    }

}
