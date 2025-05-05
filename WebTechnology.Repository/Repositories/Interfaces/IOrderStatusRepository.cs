using System.Collections.Generic;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;

namespace WebTechnology.Repository.Repositories.Interfaces
{
    public interface IOrderStatusRepository : IGenericRepository<OrderStatus>
    {
        // Có thể thêm các phương thức đặc biệt ở đây nếu cần
    }
}
