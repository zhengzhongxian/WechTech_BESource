using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Payments
{
    /// <summary>
    /// Data Transfer Object for Payment entity
    /// </summary>
    public class PaymentDTO
    {
        /// <summary>
        /// Unique identifier for the payment method
        /// </summary>
        public string PaymentId { get; set; }
        
        /// <summary>
        /// Name of the payment method
        /// </summary>
        public string PaymentName { get; set; }
        
        /// <summary>
        /// Description of the payment method
        /// </summary>
        public string Description { get; set; }
    }
}
