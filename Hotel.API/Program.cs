// //Load env
// using dotenv.net;
// using Hotel.API.Extensions;
// using Hotel.API.Middleware;
// using Hotel.Aplication.Extensions;

// using dotenv.net;
// using Hotel.API.Extensions;
// using Hotel.API.Middleware;
// using Hotel.Application.Extensions;

// DotEnv.Load();
// var builder = WebApplication.CreateBuilder(args);

// // Add Controllers
// builder.Services.AddControllers();

// // Add Services
// builder.Services.AddApplication(builder.Configuration);

// // CORS configuration
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowLocalhost",
//         builder => builder.AllowAnyOrigin()
//                           .AllowAnyMethod()
//                           .AllowAnyHeader());
// });

// // Add infrastructure services (Swagger, Authentication, Authorization, Database)
// builder.Services.AddInfrastructure(builder.Configuration);

// builder.Configuration.AddEnvironmentVariables();

// var app = builder.Build();

// // Initialize database if necessary
// await app.UseInitialiseDatabaseAsync();

// // Configure middleware for error handling and Swagger
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
// else
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
// }

// // Configure the HTTP request pipeline
// app.UseStaticFiles(); // Ensure serving static files from wwwroot

// app.UseHttpsRedirection();
// app.UseCors("AllowLocalhost");
// app.UseAuthentication();
// app.UseAuthorization();
// app.UseMiddleware<CustomExceptionHandlerMiddleware>();
// app.MapControllers();

// try
// {
//     app.Logger.LogInformation("Ứng dụng đang khởi động...");
//     app.Run();
// }
// catch (Exception ex)
// {
//     app.Logger.LogError(ex, "Đã xảy ra lỗi khi khởi động ứng dụng.");
// }
using dotenv.net;
using Hotel.API.Extensions;
using Hotel.API.Middleware;
using Hotel.Aplication.Extensions;

DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// Add Services
builder.Services.AddApplication(builder.Configuration);

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Add infrastructure services (Swagger, Authentication, Authorization, Database)
builder.Services.AddInfrastructure(builder.Configuration);

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Initialize database if necessary
await app.UseInitialiseDatabaseAsync();

// Configure middleware for error handling and Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Configure the HTTP request pipeline
app.UseStaticFiles(); // Cấu hình cơ bản cho wwwroot

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CustomExceptionHandlerMiddleware>();
app.MapControllers();

try
{
    app.Logger.LogInformation("Ứng dụng đang khởi động...");
    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Đã xảy ra lỗi khi khởi động ứng dụng.");
}