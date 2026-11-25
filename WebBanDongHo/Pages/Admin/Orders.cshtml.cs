using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebBanDongHo.Data;
using WebBanDongHo.Models;

namespace WebBanDongHo.Pages.Admin
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
            if (_context.DonHangs != null)
            {
                DonHangs = await _context.DonHangs
                    .OrderByDescending(d => d.NgayDat)
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string status)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang != null && !string.IsNullOrWhiteSpace(status))
            {
                donHang.TrangThai = status.Trim();
                _context.DonHangs.Update(donHang);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}