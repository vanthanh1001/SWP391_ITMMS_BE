using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
builder.Services.AddScoped<IJwtService, JwtService>();

// Cấu hình JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForITMMS2024WithAtLeast32Characters!";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "ITMMS-API",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "ITMMS-Client",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ITMMS API", Version = "v1" });
    
    // Thêm định nghĩa JWT authentication cho Swagger
    c.AddSecurityDefinition("Bearer", new() 
    {
        Description = "JWT Authorization header using the Bearer scheme. Nhập 'Bearer' [space] và sau đó nhập token của bạn trong text input bên dưới.\r\n\r\nVí dụ: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new()
    {
        {
            new ()
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

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
app.UseAuthentication();
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