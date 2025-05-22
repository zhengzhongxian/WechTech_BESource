using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class ApplyVoucherRepository : GenericRepository<ApplyVoucher>, IApplyVoucherRepository
    {
        private readonly WebTech _context;

        public ApplyVoucherRepository(WebTech context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApplyVoucher>> GetByOrderIdAsync(string orderId)
        {
            return await _context.ApplyVouchers
                .Include(av => av.Voucher)
                .Where(av => av.Orderid == orderId)
                .ToListAsync();
        }

        public async Task<bool> ApplyVoucherToOrderAsync(string orderId, string voucherId)
        {
            try
            {
                // Verificar si el voucher ya está aplicado a esta orden
                var existingApply = await _context.ApplyVouchers
                    .FirstOrDefaultAsync(av => av.Orderid == orderId && av.Voucherid == voucherId);

                if (existingApply != null)
                {
                    return true; // Ya está aplicado
                }

                // Verificar si el voucher existe y está activo
                var voucher = await _context.Vouchers
                    .FirstOrDefaultAsync(v => v.Voucherid == voucherId && v.IsActive == true);

                if (voucher == null)
                {
                    return false; // Voucher no existe o no está activo
                }

                // Verificar si el voucher está dentro de su período de validez
                var now = DateTime.UtcNow;
                if (voucher.StartDate > now || voucher.EndDate < now)
                {
                    return false; // Voucher fuera de período de validez
                }

                // Verificar si el voucher ha alcanzado su límite de uso
                if (voucher.UsageLimit.HasValue && voucher.UsedCount >= voucher.UsageLimit)
                {
                    return false; // Voucher ha alcanzado su límite de uso
                }

                // Verificar si el voucher no es un voucher raíz (IsRoot = false)
                if (voucher.IsRoot == true)
                {
                    return false; // No se puede aplicar un voucher raíz
                }

                // Crear nueva aplicación de voucher
                var applyVoucher = new ApplyVoucher
                {
                    Applyid = Guid.NewGuid().ToString(),
                    Orderid = orderId,
                    Voucherid = voucherId,
                    CreatedAt = DateTime.UtcNow
                };

                // Incrementar el contador de uso del voucher
                voucher.UsedCount = (voucher.UsedCount ?? 0) + 1;
                _context.Vouchers.Update(voucher);

                await _context.ApplyVouchers.AddAsync(applyVoucher);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveVoucherFromOrderAsync(string orderId, string voucherId)
        {
            try
            {
                var applyVoucher = await _context.ApplyVouchers
                    .FirstOrDefaultAsync(av => av.Orderid == orderId && av.Voucherid == voucherId);

                if (applyVoucher == null)
                {
                    return false; // No existe la aplicación del voucher
                }

                // Decrementar el contador de uso del voucher
                var voucher = await _context.Vouchers.FindAsync(voucherId);
                if (voucher != null && voucher.UsedCount > 0)
                {
                    voucher.UsedCount--;
                    _context.Vouchers.Update(voucher);
                }

                _context.ApplyVouchers.Remove(applyVoucher);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
