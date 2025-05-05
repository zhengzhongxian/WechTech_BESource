using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.Repositories.Interfaces;

namespace WebTechnology.Repository.Repositories.Implementations
{
    public class OrderStatusRepository : GenericRepository<OrderStatus>, IOrderStatusRepository
    {
        private readonly WebTech _webTech;
        
        public OrderStatusRepository(WebTech webTech) : base(webTech)
        {
            _webTech = webTech;
        }
    }
}
