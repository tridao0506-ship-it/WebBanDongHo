using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace WebBanDongHo.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly WebBanDongHo.Data.AppDbContext _context;

        public LoginModel(WebBanDongHo.Data.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
        public string TenDangNhap { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = _context.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == TenDangNhap && u.MatKhau == MatKhau);

            if (user == null)
            {
                ErrorMessage = "Tài khoản hoặc mật khẩu không chính xác!";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.TenDangNhap),
                new Claim("HoTen", user.HoTen ?? ""),
                new Claim(ClaimTypes.Role, user.VaiTro ?? "KhachHang")
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            if (user.VaiTro == "Admin")
            {
                return RedirectToPage("/Admin/Index");
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
    }
}