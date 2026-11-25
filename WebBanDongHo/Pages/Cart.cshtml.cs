using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBanDongHo.Data;
using WebBanDongHo.Extensions;
using WebBanDongHo.Models;

namespace WebBanDongHo.Pages
{
    public class CartModel : PageModel
    {
        private readonly AppDbContext _context;

        public CartModel(AppDbContext context)
        {
            _context = context;
        }

        public List<CartItem> Cart { get; set; } = new List<CartItem>();
        public decimal TongTien => Cart.Sum(item => item.GiaBan * item.SoLuong);

        public IActionResult OnGet(int? id, int quantity = 1)
        {
            Cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (id.HasValue)
            {
                var product = _context.DongHos.Find(id.Value);
                if (product != null)
                {
                    var cartItem = Cart.FirstOrDefault(c => c.Id == id.Value);
                    if (cartItem == null)
                    {
                        Cart.Add(new CartItem
                        {
                            Id = product.Id,
                            TenDongHo = product.TenDongHo,
                            GiaBan = product.GiaBan,
                            HinhAnh = product.HinhAnh,
                            SoLuong = quantity
                        });
                    }
                    else
                    {
                        cartItem.SoLuong += quantity;
                    }
                    HttpContext.Session.SetObjectAsJson("Cart", Cart);
                }
            }
            return Page();
        }

        public IActionResult OnGetIncrease(int id)
        {
            Cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = Cart.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                item.SoLuong++;
                HttpContext.Session.SetObjectAsJson("Cart", Cart);
            }
            return RedirectToPage();
        }

        public IActionResult OnGetDecrease(int id)
        {
            Cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = Cart.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                item.SoLuong--;
                if (item.SoLuong <= 0)
                {
                    Cart.Remove(item);
                }
                HttpContext.Session.SetObjectAsJson("Cart", Cart);
            }
            return RedirectToPage();
        }

        public IActionResult OnGetRemove(int id)
        {
            Cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var itemToRemove = Cart.FirstOrDefault(c => c.Id == id);
            if (itemToRemove != null)
            {
                Cart.Remove(itemToRemove);
                HttpContext.Session.SetObjectAsJson("Cart", Cart);
            }
            return RedirectToPage();
        }
    }
}