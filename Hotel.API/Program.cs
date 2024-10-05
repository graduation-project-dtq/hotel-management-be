//Load env
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
// Add cors, swagger, authentication, and authorization, database
builder.Services.AddInfrastructure(builder.Configuration);

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

await app.UseInitialiseDatabaseAsync();

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
// Configure the HTTP request pipeline.

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