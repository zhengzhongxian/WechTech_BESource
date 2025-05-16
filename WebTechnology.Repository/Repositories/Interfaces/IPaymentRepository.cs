using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    /// <summary>
    /// Interface for Payment repository operations
    /// </summary>
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        // We can add specific methods for Payment entity here if needed
        // For now, we'll use the methods from IGenericRepository
    }
}
