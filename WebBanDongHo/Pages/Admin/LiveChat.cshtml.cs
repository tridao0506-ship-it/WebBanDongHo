using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebBanDongHo.Data;
using WebBanDongHo.Models;

namespace WebBanDongHo.Pages.Admin
{
    [IgnoreAntiforgeryToken]
    public class LiveChatModel : PageModel
    {
        private readonly AppDbContext _context;

        public LiveChatModel(AppDbContext context)
        {
            _context = context;
        }

        public List<KhachHangChatViewModel> DanhSachKhachHang { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name != "admin")
            {
                return RedirectToPage("/Account/Login");
            }

            var users = await _context.NguoiDungs
                .Where(u => u.TenDangNhap != "admin")
                .ToListAsync();

            foreach (var u in users)
            {
                int chuaDoc = await _context.TinNhans
                    .CountAsync(t => t.NguoiGui == u.TenDangNhap && t.NguoiNhan == "admin" && !t.DaDoc);

                DanhSachKhachHang.Add(new KhachHangChatViewModel
                {
                    TenDangNhap = u.TenDangNhap,
                    HoTen = u.TenDangNhap,
                    Email = u.Email,
                    TinNhanChuaDoc = chuaDoc
                });
            }

            return Page();
        }

        public async Task<IActionResult> OnGetMessagesAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return new JsonResult(new List<object>());

            var unread = await _context.TinNhans
                .Where(t => t.NguoiGui == username && t.NguoiNhan == "admin" && !t.DaDoc)
                .ToListAsync();

            if (unread.Any())
            {
                foreach (var msg in unread) msg.DaDoc = true;
                await _context.SaveChangesAsync();
            }

            var messages = await _context.TinNhans
                .Where(t => (t.NguoiGui == username && t.NguoiNhan == "admin") || (t.NguoiGui == "admin" && t.NguoiNhan == username))
                .OrderBy(t => t.ThoiGian)
                .Select(t => new {
                    sender = t.NguoiGui,
                    message = t.NoiDung
                })
                .ToListAsync();

            return new JsonResult(messages);
        }

        public async Task<IActionResult> OnPostSendAsync([FromBody] ChatInputModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Message))
            {
                return new JsonResult(new { success = false });
            }

            var tinMoi = new TinNhan
            {
                NguoiGui = "admin",
                NguoiNhan = model.Username,
                NoiDung = model.Message,
                ThoiGian = DateTime.Now,
                DaDoc = false
            };

            _context.TinNhans.Add(tinMoi);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }

    public class KhachHangChatViewModel
    {
        public string TenDangNhap { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TinNhanChuaDoc { get; set; }
    }

    public class ChatInputModel
    {
        public string Username { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}