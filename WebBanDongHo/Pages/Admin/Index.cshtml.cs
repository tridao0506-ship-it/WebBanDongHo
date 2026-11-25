using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using WebBanDongHo.Data;
using WebBanDongHo.Models;

namespace WebBanDongHo.Pages.Admin
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<DongHo> DongHo { get; set; } = new List<DongHo>();
        public IList<DonHang> DonHangs { get; set; } = new List<DonHang>();
        public IList<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();

        public decimal TongDoanhThu { get; set; }
        public decimal DoanhThuHoanThanh { get; set; }
        public int TongSoDonHang { get; set; }
        public int SoDonHoanThanh { get; set; }

        public int SoDonDangGiao { get; set; }
        public int SoDonDaHuy { get; set; }
        public string ChartLabelsJson { get; set; } = "[]";
        public string ChartDataJson { get; set; } = "[]";

        public async Task OnGetAsync()
        {
            if (_context.DongHos != null)
            {
                DongHo = await _context.DongHos.OrderBy(d => d.SoLuongTon).AsNoTracking().ToListAsync();
            }

            if (_context.DonHangs != null)
            {
                DonHangs = await _context.DonHangs
                    .Include(d => d.ChiTietDonHangs)
                    .OrderByDescending(d => d.NgayDat)
                    .AsNoTracking()
                    .ToListAsync();

                TongDoanhThu = DonHangs
                    .Where(d => d.TrangThai == null || (d.TrangThai.Trim() != "Đã hủy" && d.TrangThai.Trim() != "Hủy"))
                    .Sum(d => d.TongTien);

                DoanhThuHoanThanh = DonHangs
                    .Where(d => d.TrangThai != null && d.TrangThai.Trim() == "Hoàn thành")
                    .Sum(d => d.TongTien);

                TongSoDonHang = DonHangs.Count;
                SoDonHoanThanh = DonHangs.Count(d => d.TrangThai != null && d.TrangThai.Trim() == "Hoàn thành");
                SoDonDangGiao = DonHangs.Count(d => d.TrangThai != null && d.TrangThai.Trim() == "Đang giao");
                SoDonDaHuy = DonHangs.Count(d => d.TrangThai != null && (d.TrangThai.Trim() == "Đã hủy" || d.TrangThai.Trim() == "Hủy"));

                var doanhThuTheoNgay = DonHangs
                    .Where(d => d.TrangThai == null || (d.TrangThai.Trim() != "Đã hủy" && d.TrangThai.Trim() != "Hủy"))
                    .OrderBy(d => d.NgayDat)
                    .GroupBy(d => d.NgayDat.ToString("dd/MM/yyyy"))
                    .Select(g => new
                    {
                        Ngay = g.Key,
                        Tong = g.Sum(x => x.TongTien)
                    })
                    .ToList();

                ChartLabelsJson = JsonSerializer.Serialize(doanhThuTheoNgay.Select(x => x.Ngay));
                ChartDataJson = JsonSerializer.Serialize(doanhThuTheoNgay.Select(x => x.Tong));
            }

            if (_context.NguoiDungs != null)
            {
                NguoiDungs = await _context.NguoiDungs.AsNoTracking().ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostQuickUpdateStockAsync(int id, int newStock)
        {
            var dh = await _context.DongHos.FindAsync(id);
            if (dh != null)
            {
                dh.SoLuongTon = newStock < 0 ? 0 : newStock;
                _context.DongHos.Update(dh);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string status)
        {
            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donHang != null && donHang.TrangThai != status)
            {
                string oldStatus = donHang.TrangThai?.Trim() ?? "";

                if (oldStatus != "Đã hủy" && status == "Đã hủy")
                {
                    await CapNhatKhoTheoDonHang(donHang, congKho: true);
                }
                else if (oldStatus == "Đã hủy" && status != "Đã hủy")
                {
                    await CapNhatKhoTheoDonHang(donHang, congKho: false);
                }

                donHang.TrangThai = status;
                _context.DonHangs.Update(donHang);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        private async Task CapNhatKhoTheoDonHang(DonHang donHang, bool congKho)
        {
            var danhSachDongHo = await _context.DongHos.ToListAsync();

            if (donHang.ChiTietDonHangs != null && donHang.ChiTietDonHangs.Any())
            {
                foreach (var ct in donHang.ChiTietDonHangs)
                {
                    var dh = danhSachDongHo.FirstOrDefault(x => x.Id == ct.DongHoId);
                    if (dh != null)
                    {
                        dh.SoLuongTon += congKho ? ct.SoLuong : -ct.SoLuong;
                        if (dh.SoLuongTon < 0) dh.SoLuongTon = 0;
                        _context.DongHos.Update(dh);
                    }
                }
            }
            else
            {
                var dh = danhSachDongHo.FirstOrDefault(x => Math.Abs(x.GiaBan - donHang.TongTien) < 1);
                if (dh != null)
                {
                    dh.SoLuongTon += congKho ? 1 : -1;
                    if (dh.SoLuongTon < 0) dh.SoLuongTon = 0;
                    _context.DongHos.Update(dh);
                }
            }
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(int id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user != null && user.TenDangNhap != "admin")
            {
                _context.NguoiDungs.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetExportExcelAsync()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name != "admin")
            {
                return RedirectToPage("/Account/Login");
            }

            var donHangs = await _context.DonHangs.OrderByDescending(d => d.NgayDat).ToListAsync();

            var builder = new StringBuilder();
            builder.Append("\uFEFF");
            builder.AppendLine("Mã đơn;Ngày đặt;Khách hàng;Số điện thoại;Địa chỉ giao;Tổng tiền (VNĐ);Trạng thái");

            foreach (var item in donHangs)
            {
                string diaChi = item.DiaChi?.Replace(";", " ") ?? "";
                string hoTen = item.HoTen?.Replace(";", " ") ?? "";
                string trangThai = item.TrangThai?.Replace(";", " ") ?? "";

                builder.AppendLine($"#{item.Id};{item.NgayDat:dd/MM/yyyy HH:mm};{hoTen};{item.SoDienThoai};\"{diaChi}\";{item.TongTien};{trangThai}");
            }

            byte[] fileBytes = Encoding.UTF8.GetBytes(builder.ToString());
            string fileName = $"BaoCaoDoanhThu_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            return File(fileBytes, "text/csv; charset=utf-8", fileName);
        }
    }
}