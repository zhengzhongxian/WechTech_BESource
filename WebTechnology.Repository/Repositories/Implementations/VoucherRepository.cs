using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class VoucherRepository : GenericRepository<Voucher>, IVoucherRepository
    {
        private readonly WebTech _webTech;
        public VoucherRepository(WebTech webTech) : base(webTech)
        {
            _webTech = webTech;
        }

        public async Task<IEnumerable<Voucher>> FindAsync(Expression<Func<Voucher, bool>> predicate)
        {
            return await _webTech.Vouchers
                .Where(predicate)
                .ToListAsync();
        }
    }
}
