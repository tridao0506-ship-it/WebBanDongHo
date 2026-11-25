using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebBanDongHo.Data;
using WebBanDongHo.Extensions;
using WebBanDongHo.Models;
using WebBanDongHo.Services;

namespace WebBanDongHo.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly GhnShippingService _ghnService;

        public CheckoutModel(AppDbContext context, GhnShippingService ghnService)
        {
            _context = context;
            _ghnService = ghnService;
        }

        public List<CartItem> Cart { get; set; } = new List<CartItem>();
        public decimal TienHang => Cart.Sum(item => item.GiaBan * item.SoLuong);
        public decimal TongTien => TienHang + PhiShip;

        [BindProperty]
        public decimal PhiShip { get; set; } = 0;

        [BindProperty]
        public string PhuongThucThanhToan { get; set; } = "COD";

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        public string HoTen { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ. Phải bắt đầu bằng số 0 và có chính xác 10 chữ số.")]
        public string SoDienThoai { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
        public string DiaChi { get; set; } = string.Empty;

        private bool IsCustomerAuthenticated()
        {
            return User?.Identity != null && User.Identity.IsAuthenticated;
        }

        private IActionResult RedirectToLogin()
        {
            var returnUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
            return RedirectToPage("/Account/Login", new { returnUrl });
        }

        public IActionResult OnGet()
        {
            if (!IsCustomerAuthenticated())
            {
                return RedirectToLogin();
            }

            Cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            if (!Cart.Any())
            {
                return RedirectToPage("/Cart");
            }
            return Page();
        }

        public async Task<IActionResult> OnGetGhnProvincesAsync()
        {
            var json = await _ghnService.GetProvincesAsync();
            return Content(json, "application/json");
        }

        public async Task<IActionResult> OnGetGhnDistrictsAsync(int provinceId)
        {
            var json = await _ghnService.GetDistrictsAsync(provinceId);
            return Content(json, "application/json");
        }

        public async Task<IActionResult> OnGetGhnWardsAsync(int districtId)
        {
            var json = await _ghnService.GetWardsAsync(districtId);
            return Content(json, "application/json");
        }

        public record ShippingFeeRequest(int ToDistrictId, string ToWardCode);

        public async Task<IActionResult> OnPostCalculateFeeAsync([FromBody] ShippingFeeRequest req)
        {
            var currentCart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var totalWeight = currentCart.Sum(c => c.SoLuong * 250);
            if (totalWeight < 300) totalWeight = 300;

            var fee = await _ghnService.CalculateFeeAsync(req.ToDistrictId, req.ToWardCode, totalWeight);
            return new JsonResult(new { fee });
        }

        public IActionResult OnPost()
        {
            if (!IsCustomerAuthenticated())
            {
                return RedirectToLogin();
            }

            Cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            if (!Cart.Any())
            {
                return RedirectToPage("/Cart");
            }

            if (PhiShip < 0)
            {
                PhiShip = 0;
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var trangThai = PhuongThucThanhToan == "VietQR"
                ? "Chờ xác nhận chuyển khoản (QR)"
                : "Đang xử lý (COD)";

            var donHang = new DonHang
            {
                HoTen = HoTen,
                SoDienThoai = SoDienThoai,
                DiaChi = DiaChi,
                NgayDat = DateTime.UtcNow,
                TrangThai = trangThai,
                TongTien = TongTien,
                ChiTietDonHangs = Cart.Select(c => new ChiTietDonHang
                {
                    DongHoId = c.Id,
                    SoLuong = c.SoLuong,
                    GiaBan = c.GiaBan
                }).ToList()
            };

            _context.DonHangs.Add(donHang);

            foreach (var item in Cart)
            {
                var dongHoInDb = _context.DongHos.Find(item.Id);
                if (dongHoInDb != null)
                {
                    dongHoInDb.SoLuongTon -= item.SoLuong;
                    if (dongHoInDb.SoLuongTon < 0)
                    {
                        dongHoInDb.SoLuongTon = 0;
                    }
                }
            }

            _context.SaveChanges();
            HttpContext.Session.Remove("Cart");

            return RedirectToPage("/OrderSuccess");
        }
    }
}