//using Hotel_API;
//using Microsoft.OpenApi.Models;

//var builder = WebApplication.CreateBuilder(args);

//// Gọi phương thức AddConfig để đăng ký các dịch vụ từ DependencyInjection
//builder.Services.AddConfig(builder.Configuration);

//builder.Services.AddSwaggerGen(c => {
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "JWTToken_Auth_API",
//        Version = "v1"
//    });
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
//    });
//    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
//        {
//            new OpenApiSecurityScheme {
//                Reference = new OpenApiReference {
//                    Type = ReferenceType.SecurityScheme,
//                        Id = "Bearer"
//                }
//            },
//            new string[] {}
//        }
//    });
//});

//// Cấu hình AutoMapper
//builder.Services.AddAutoMapper(typeof(Program));
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();
//app.UseAuthentication();
//app.Run();

using Hotel_API;
using Microsoft.OpenApi.Models;

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
builder.Services.AddAutoMapper(typeof(Program));

// Thêm các dịch vụ mặc định
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Chỉ hiển thị Swagger trong môi trường phát triển (development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWTToken_Auth_API v1");
    });
}

// Cấu hình các middleware
app.UseHttpsRedirection();

// Đặt UseAuthentication trước UseAuthorization
app.UseAuthentication();  // Xác thực người dùng trước
app.UseAuthorization();   // Kiểm tra quyền truy cập

app.MapControllers();

// Chạy ứng dụng
app.Run();
