using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.Payments;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    /// <summary>
    /// Implementation of the Payment service
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IMapper mapper,
            ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all payment methods
        /// </summary>
        /// <returns>Service response containing list of payment methods</returns>
        public async Task<ServiceResponse<IEnumerable<PaymentDTO>>> GetAllPaymentsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all payment methods");
                
                // Get all payments from repository
                var payments = await _paymentRepository.GetAllAsync();
                
                // Map to DTOs
                var paymentDTOs = _mapper.Map<IEnumerable<PaymentDTO>>(payments);
                
                return new ServiceResponse<IEnumerable<PaymentDTO>>
                {
                    Data = paymentDTOs,
                    Message = "Lấy danh sách phương thức thanh toán thành công",
                    Success = true,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all payment methods");
                
                return new ServiceResponse<IEnumerable<PaymentDTO>>
                {
                    Message = "Đã xảy ra lỗi khi lấy danh sách phương thức thanh toán",
                    Success = false,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
