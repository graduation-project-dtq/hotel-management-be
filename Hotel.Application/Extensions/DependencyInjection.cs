using System.Reflection;
using Hotel.Application.Interfaces;
using Hotel.Application.Services;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Hotel.Infrastructure.IOW;
using Hotel.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Hotel.Aplication.Extensions
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepository();
            services.AddService(configuration);
            services.AddAutoMapper();
        }


        //Đăng ký repository
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }

        //Đăng ký service
        public static void AddService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            // Đăng ký AuthService
            services.AddScoped<IAuthService, AuthService>();

            // Đăng ký TokenService (nếu cần)
            services.AddScoped<ITokenService, TokenService>();

            //Customer
            services.AddScoped<ICustomerService,CustomerService>();

            //Employee
            services.AddScoped<IEmployeeService, EmployeeService>();
            
            //RoomType
            services.AddScoped<IRoomTypeService, RoomTypeService>();
        }

        //Đăng ký mapper
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}