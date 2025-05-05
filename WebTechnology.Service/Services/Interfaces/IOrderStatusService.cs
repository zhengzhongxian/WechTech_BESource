using System.Collections.Generic;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.DTOs.OrderStatus;
using WebTechnology.Service.Models;

namespace WebTechnology.Service.Services.Interfaces
{
    public interface IOrderStatusService
    {
        Task<ServiceResponse<IEnumerable<OrderStatusDTO>>> GetAllOrderStatusAsync();
    }
}
