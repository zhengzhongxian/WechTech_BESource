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
    /// <summary>
    /// Implementation of the Payment repository
    /// </summary>
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly WebTech _context;
        
        public PaymentRepository(WebTech context) : base(context)
        {
            _context = context;
        }
        
        // We can add specific implementation methods for Payment entity here if needed
    }
}
