using Microsoft.EntityFrameworkCore;
using NguyenDucHuy_2123110217_ASP.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình Database - Quan trọng: Render cần Connection String này ở tab Environment
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. SỬA TẠI ĐÂY: Bỏ điều kiện if (app.Environment.IsDevelopment()) để Swagger chạy trên Render
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; // Để khi vào link web là hiện thẳng Swagger luôn, không cần gõ /swagger
});

// Render thường xử lý HTTPS ở lớp ngoài, nên đôi khi dòng này gây lỗi vòng lặp nếu không cấu hình kỹ.
// Nếu bạn vào web vẫn lỗi, hãy thử tạm thời comment dòng dưới lại.
app.UseHttpsRedirection();


app.UseAuthorization();
app.MapControllers();

app.Run();