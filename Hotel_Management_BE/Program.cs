using Hotel_API;

var builder = WebApplication.CreateBuilder(args);

// Gọi phương thức AddConfig để đăng ký các dịch vụ từ DependencyInjection
builder.Services.AddConfig(builder.Configuration);
// Cấu hình AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
