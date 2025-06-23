using Microsoft.EntityFrameworkCore;
using SWP391_ITMMS_Api.Data;
using SWP391_ITMMS_Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Thêm Swagger services
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Bật Swagger UI ở mọi môi trường
app.UseSwagger();
app.UseSwaggerUI();

// Sử dụng CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// API endpoint mẫu để test
app.MapGet("/", () => "Welcome to ITMMS API - Hệ thống quản lý và theo dõi điều trị hiếm muộn!");

app.MapGet("/api/health", () => new 
{ 
    status = "healthy", 
    timestamp = DateTime.Now,
    version = "1.0.0",
    database = "connected"
});

app.Run();