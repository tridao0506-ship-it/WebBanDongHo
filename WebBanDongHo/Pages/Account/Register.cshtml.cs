using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBanDongHo.Models;

namespace WebBanDongHo.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly WebBanDongHo.Data.AppDbContext _context;

        public RegisterModel(WebBanDongHo.Data.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public NguoiDung NguoiDungInfo { get; set; } = new NguoiDung();

        [BindProperty]
        public string XacNhanMatKhau { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            if (NguoiDungInfo.MatKhau != XacNhanMatKhau)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp!";
                return Page();
            }

            bool userExists = _context.NguoiDungs.Any(u => u.TenDangNhap == NguoiDungInfo.TenDangNhap);
            if (userExists)
            {
                ErrorMessage = "Tên đăng nhập này đã được sử dụng!";
                return Page();
            }

            _context.NguoiDungs.Add(NguoiDungInfo);
            _context.SaveChanges();

            return RedirectToPage("/Account/Login");
        }
    }
}