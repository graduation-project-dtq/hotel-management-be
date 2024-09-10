using Hotel.Contract.Repositories.Entity;
using Hotel.Repositories.Context;
using Hotel.Services;
using Hotel.Services.Service;
using Hotel.Contract.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hotel.Contract.Repositories.IUOW;
using Hotel.Repositories.UOW;
using Hotel.Core.App;

namespace Hotel_API
{
    public static class DependencyInjection
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigRoute();
            services.AddDatabase(configuration);
            services.AddIdentity();
            services.AddServices();
            services.AddModifiedAuthentication(configuration);
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
            services.AddIdentity<User, IdentityRole>(options =>
            {
                // Cấu hình tùy chọn Identity
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddRoleManager<RoleManager<IdentityRole>>()  // Đăng ký RoleManager
            .AddDefaultTokenProviders();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IRoomCategoryService, RoomCategoryService>();
            services.AddScoped<IRoomTypeDetailService, RoomTypeDetailService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void AddModifiedAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["JWT:ValidIssuer"],
                        ValidAudience = configuration["JWT:ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole(AppRole.Administrator));
                options.AddPolicy("RequireDefaultRole", policy => policy.RequireRole(AppRole.DefaultRole));
                options.AddPolicy("RequireCustomerRole", policy => policy.RequireRole(AppRole.Customer));
            });
        }
    }
}
