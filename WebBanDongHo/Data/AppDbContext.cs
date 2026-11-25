using Microsoft.EntityFrameworkCore;
using WebBanDongHo.Models;

namespace WebBanDongHo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DongHo> DongHos { get; set; }

        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

        public DbSet<NguoiDung> NguoiDungs { get; set; }

        public DbSet<WebBanDongHo.Models.TinNhan> TinNhans { get; set; }
    }
}