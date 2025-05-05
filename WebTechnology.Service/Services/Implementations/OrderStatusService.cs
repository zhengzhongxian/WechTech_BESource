using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.OrderStatus;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Service.Services.Implementations
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly IOrderStatusRepository _orderStatusRepository;
        private readonly IMapper _mapper;

        public OrderStatusService(IOrderStatusRepository orderStatusRepository, IMapper mapper)
        {
            _orderStatusRepository = orderStatusRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<IEnumerable<OrderStatusDTO>>> GetAllOrderStatusAsync()
        {
            try
            {
                var orderStatuses = await _orderStatusRepository.GetAllAsync();
                var orderStatusDTOs = _mapper.Map<IEnumerable<OrderStatusDTO>>(orderStatuses);
                
                return ServiceResponse<IEnumerable<OrderStatusDTO>>.SuccessResponse(
                    orderStatusDTOs, 
                    "Lấy danh sách trạng thái đơn hàng thành công"
                );
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<OrderStatusDTO>>.ErrorResponse(
                    $"Lỗi khi lấy danh sách trạng thái đơn hàng: {ex.Message}"
                );
            }
        }
    }
}
