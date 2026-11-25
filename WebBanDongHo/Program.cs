using Microsoft.EntityFrameworkCore;
using WebBanDongHo.Hubs;
using WebBanDongHo.Services;

namespace WebBanDongHo;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<WebBanDongHo.Data.AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddRazorPages();
        builder.Services.AddSession();

        builder.Services.AddSignalR();

        builder.Services.AddHttpClient<GhnShippingService>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

        builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", options =>
        {
            options.Cookie.Name = "WebBanDongHoCookie";
            options.LoginPath = "/Account/Login";
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseSession();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        app.MapHub<ChatHub>("/chatHub");

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<WebBanDongHo.Data.AppDbContext>();
            context.Database.EnsureCreated();

            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS ""TinNhans"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""NguoiGui"" TEXT NOT NULL,
                    ""NguoiNhan"" TEXT NOT NULL,
                    ""NoiDung"" TEXT NOT NULL,
                    ""ThoiGian"" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
                    ""DaDoc"" BOOLEAN NOT NULL
                );
            ");

            if (!context.NguoiDungs.Any(u => u.TenDangNhap == "admin"))
            {
                context.NguoiDungs.Add(new WebBanDongHo.Models.NguoiDung
                {
                    TenDangNhap = "admin",
                    MatKhau = "123",
                    HoTen = "Qu?n Tr? Viên",
                    Email = "admin@gmail.com",
                    VaiTro = "Admin"
                });
                context.SaveChanges();
            }
        }

        app.Run();
    }
}