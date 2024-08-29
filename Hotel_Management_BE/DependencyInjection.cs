using Hotel.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using Hotel.Services;
using Hotel.Contract.Services;
using Hotel.Services.Service;

namespace Hotel_API
{
    public static class DependencyInjection
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigRoute();
            services.AddDatabase(configuration);
            services.AddIdentity();
            services.AddInfrastructure(configuration);
            services.AddServices();
        }

        public static void ConfigRoute(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
        }

        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseLazyLoadingProxies()
                       .UseSqlServer(configuration.GetConnectionString("DbConnect"), b => b.MigrationsAssembly("Hotel_API"));
            });
        }



        public static void AddIdentity(this IServiceCollection services)
        {
            
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IRoomService, RoomService>();
        }
    }
}
