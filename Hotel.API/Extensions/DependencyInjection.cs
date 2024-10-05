using Hotel.Core.Base;
using Hotel.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Hotel.API.Extensions
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSetting(services, configuration);
            ConfigureCors(services);
            ConfigureAuthentication(services, configuration);
            AddDatabases(services, configuration);
            AddCustomHttpClient(services); // Thêm HttpClient
            AddSwagger(services);
            AddInitialiseDatabase(services);
        }

        public static void AddCustomHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient("CustomHttpClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7227/api"); // Địa chỉ base URL của API
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true // Bỏ qua xác thực SSL
                };
            });
        }

        // JWT Setting
        public static void JwtSetting(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(option =>
            {
                JwtSettings jwtSettings = new()
                {
                    SecretKey = configuration["JWT_KEY"],
                    Issuer = configuration["JWT_ISSUER"],
                    Audience = configuration["JWT_AUDIENCE"],
                    AccessTokenExpirationMinutes = configuration.GetValue<int>("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES"),
                    RefreshTokenExpirationDays = configuration.GetValue<int>("JWT_REFRESH_TOKEN_EXPIRATION_DAYS")
                };
                jwtSettings.IsValid();
                return jwtSettings;
            });
        }

        // Configure Cors
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", builder =>
                {
                    builder.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002", "http://localhost:5000", "http://0.0.0.0:7227", "http://192.168.1.12")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }

        // Configure Authentication 
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT_ISSUER"],
                    ValidAudience = configuration["JWT_AUDIENCE"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT_KEY"])),
                    ClockSkew = TimeSpan.FromMinutes(0) // Không có độ trễ thời gian
                };
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
            });
        }

        // Database
        public static void AddDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                services.AddDbContext<HotelDBContext>(options =>
                    options.UseSqlServer(
                        Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"),
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly("Hotel.Infrastructure"); // Cấu hình MigrationsAssembly ở đây
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 10,
                                maxRetryDelay: TimeSpan.FromSeconds(60),
                                errorNumbersToAdd: null);
                        }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddDatabases: {ex.Message}");
                throw;
            }
        }

        // Add Swagger
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Seminar Management API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }

        // Seed Data
        public static void AddInitialiseDatabase(this IServiceCollection services)
        {
            services.AddScoped<ApplicationDbContextInitialiser>();
        }

        public static async Task UseInitialiseDatabaseAsync(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            ApplicationDbContextInitialiser initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            await initialiser.InitialiseAsync();
            await initialiser.SeedAsync();
        }
    }
}
