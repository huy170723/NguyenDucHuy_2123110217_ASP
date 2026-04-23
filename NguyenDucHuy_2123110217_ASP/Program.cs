using Microsoft.EntityFrameworkCore;
using NguyenDucHuy_2123110217_ASP.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Kết nối Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Cấu hình CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 3. Fix lỗi Object Cycle (Vòng lặp JSON)
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// 3. Fix lỗi Object Cycle (Vòng lặp JSON) and enable MVC views
// 3. Fix lỗi Object Cycle và hỗ trợ cả Controller API + View MVC
builder.Services.AddControllersWithViews().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Cấu hình Swagger
if (app.Environment.IsDevelopment() || true) // Cho phép chạy Swagger cả khi deploy (nếu muốn)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger";
    });
}

// 5. Cấu hình File tĩnh (Để mở index.html)
    c.RoutePrefix = "swagger"; // Để swagger ở đường dẫn /swagger cho đỡ rối
});

// 5. Cấu hình File tĩnh (Để mở index.html)
// 5. Cấu hình File tĩnh (Để mở index.html trong wwwroot)
app.UseDefaultFiles();
app.UseStaticFiles();

// Khi deploy Render, có thể comment dòng này nếu gặp lỗi chuyển hướng HTTPS
// app.UseHttpsRedirection();

// 6. Kích hoạt Middleware
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

// Map default route cho MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 7. Seed data tự động
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Initialize(db);
}

app.Run();