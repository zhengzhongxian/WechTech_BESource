using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Payments;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    /// <summary>
    /// Interface for Payment service operations
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Get all payment methods
        /// </summary>
        /// <returns>Service response containing list of payment methods</returns>
        Task<ServiceResponse<IEnumerable<PaymentDTO>>> GetAllPaymentsAsync();
    }
}
