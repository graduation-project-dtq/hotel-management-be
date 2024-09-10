using Hotel.Contract.Repositories.Entity;
using Hotel.Repositories.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Hotel_API;

var builder = WebApplication.CreateBuilder(args);

// Gọi phương thức AddConfig để đăng ký các dịch vụ từ DependencyInjection
builder.Services.AddConfig(builder.Configuration);

// Cấu hình Swagger với bảo mật JWT
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });

    // Cấu hình để sử dụng JWT cho Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });

    // Yêu cầu JWT cho tất cả các endpoint
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Cấu hình AutoMapper
//builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Thêm các dịch vụ mặc định
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Chỉ hiển thị Swagger trong môi trường phát triển (development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request path: {context.Request.Path}");
    Console.WriteLine($"User authenticated: {context.User.Identity.IsAuthenticated}");
    if (context.User.Identity.IsAuthenticated)
    {
        Console.WriteLine($"User roles: {string.Join(", ", context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))}");
    }
    await next();
});

// Cấu hình các middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Chạy ứng dụng
app.Run();
