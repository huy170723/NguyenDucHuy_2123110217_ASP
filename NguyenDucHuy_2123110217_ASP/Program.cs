using Microsoft.EntityFrameworkCore;
using NguyenDucHuy_2123110217_ASP.Data;
using System.Text.Json.Serialization; // 1. Thêm thư viện này

var builder = WebApplication.CreateBuilder(args);

// 1. Kết nối Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Thêm CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 2. SỬA TẠI ĐÂY: Thêm IgnoreCycles để fix lỗi "object cycle"
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Cấu hình Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.UseDefaultFiles();
app.UseStaticFiles();

// Nếu chạy local bị lỗi SSL, có thể comment dòng này
app.UseHttpsRedirection();

// 4. Kích hoạt CORS (Phải để TRƯỚC MapControllers)
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();