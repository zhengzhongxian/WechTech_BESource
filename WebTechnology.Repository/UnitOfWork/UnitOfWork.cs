using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WebTech _context;
        private IDbContextTransaction _transaction;
        private IProductRepository _products;
        private IImageRepository _images;
        private IVoucherRepository _vouchers;
        private IApplyVoucherRepository _applyVouchers;

        public UnitOfWork(
            WebTech context,
            IProductRepository productRepository,
            IImageRepository imageRepository,
            IVoucherRepository voucherRepository,
            IApplyVoucherRepository applyVoucherRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _products = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _images = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
            _vouchers = voucherRepository ?? throw new ArgumentNullException(nameof(voucherRepository));
            _applyVouchers = applyVoucherRepository ?? throw new ArgumentNullException(nameof(applyVoucherRepository));
        }

        public IProductRepository Products => _products;
        public IImageRepository Images => _images;
        public IVoucherRepository Vouchers => _vouchers;
        public IApplyVoucherRepository ApplyVouchers => _applyVouchers;

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                return;

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
                return;

            try
            {
                await SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
                return;

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }

}
