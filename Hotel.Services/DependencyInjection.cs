using Hotel.Contract.Repositories.IUOW;
using Hotel.Repositories.UOW;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Hotel.Contract.Services.IService;
using Hotel.Services.Service;


namespace Hotel.Services
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepositories();
            services.AddServices(); // Đăng ký các service tại đây
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void AddServices(this IServiceCollection services)
        {
           services.AddScoped<IRoomService,RoomService>();
            services.AddScoped<IRoomCategoryService, RoomCategoryService>();
        }
    }
}
