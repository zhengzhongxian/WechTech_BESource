using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.Repositories.Implementations;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.CoreHelpers.Multimedia;
using WebTechnology.Service.Services.Implementationns;
using WebTechnology.Service.Services.Implementations;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.Configurations
{
    public static class ServiceConfiguration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            // Add HttpContextAccessor
            services.AddHttpContextAccessor();

            // Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ITrendRepository, TrendRepostiory>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductTrendsRepository, ProductTrendsRepository>();
            services.AddScoped<IDimensionRepository, DimensionRepository>();
            services.AddScoped<IProductPriceRepository, ProductPriceRepository>();
            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IProductStatusRepository, ProductStatusRespository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IStatisticsRepository, StatisticsRepository>();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IApplyVoucherRepository, ApplyVoucherRepository>();
            services.AddScoped<IUserStatusRepository, UserStatusRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IParentRepository, ParentRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Services
            services.AddScoped<ITrendService, TrendService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartItemService, CartItemService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderStatusService, OrderStatusService>();
            services.AddScoped<IProductStatusService, ProductStatusService>();
            services.AddScoped<IProductPriceService, ProductPriceService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IDimensionService, DimensionService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IUserStatusService, UserStatusService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IParentService, ParentService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<ICouponService, CouponService>();
        }
    }
}