using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebBanDongHo.Data;
using WebBanDongHo.Models;

namespace WebBanDongHo.Pages
{
    [Authorize]
    public class OrdersModel : PageModel
    {
        private readonly AppDbContext _context;

        public OrdersModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<DonHang> DonHangs { get; set; } = default!;

        public async Task OnGetAsync()
        {
            DonHangs = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCancelOrderAsync(int id)
        {
            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donHang != null && donHang.TrangThai == "?ang x? lý")
            {
                donHang.TrangThai = "?ã h?y";

                if (donHang.ChiTietDonHangs != null && donHang.ChiTietDonHangs.Any())
                {
                    foreach (var ct in donHang.ChiTietDonHangs)
                    {
                        var dongHo = await _context.DongHos.FindAsync(ct.DongHoId);
                        if (dongHo != null)
                        {
                            dongHo.SoLuongTon += ct.SoLuong;
                        }
                    }
                }
                else
                {
                    var dongHo = await _context.DongHos.FirstOrDefaultAsync(d => d.GiaBan == donHang.TongTien);
                    if (dongHo != null)
                    {
                        dongHo.SoLuongTon += 1;
                    }
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}