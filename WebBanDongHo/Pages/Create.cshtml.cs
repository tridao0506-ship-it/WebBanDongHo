using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBanDongHo.Data;
using System.Text.Json;

namespace WebBanDongHo.Pages
{
    public class ShoppingCartModel : PageModel
    {
        private readonly AppDbContext _db;

        public ShoppingCartModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<ShoppingCartItem> Cart { get; set; } = new();

        public void OnGet()
        {
            Cart = LoadCart();
        }

        public IActionResult OnGetAddToCart(int id)
        {
            var product = _db.DongHos.FirstOrDefault(p => p.Id == id);
            if (product == null) return RedirectToPage("/Index");

            Cart = LoadCart();

            var item = Cart.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                Cart.Add(new ShoppingCartItem
                {
                    Id = product.Id,
                    TenDongHo = product.TenDongHo,
                    GiaBan = product.GiaBan,
                    HinhAnh = product.HinhAnh,
                    SoLuong = 1
                });
            }
            else
            {
                item.SoLuong++;
            }

            SaveCart(Cart);
            return RedirectToPage();
        }

        public IActionResult OnGetIncrease(int id)
        {
            Cart = LoadCart();
            var item = Cart.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                item.SoLuong++;
                SaveCart(Cart);
            }
            return RedirectToPage();
        }

        public IActionResult OnGetDecrease(int id)
        {
            Cart = LoadCart();
            var item = Cart.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                item.SoLuong--;
                if (item.SoLuong <= 0) Cart.Remove(item);
                SaveCart(Cart);
            }
            return RedirectToPage();
        }

        public IActionResult OnGetDelete(int id)
        {
            Cart = LoadCart();
            var item = Cart.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                Cart.Remove(item);
                SaveCart(Cart);
            }
            return RedirectToPage();
        }

        private List<ShoppingCartItem> LoadCart()
        {
            var sessionData = HttpContext.Session.GetString("MyCartSession");
            if (string.IsNullOrEmpty(sessionData)) return new List<ShoppingCartItem>();
            return JsonSerializer.Deserialize<List<ShoppingCartItem>>(sessionData) ?? new List<ShoppingCartItem>();
        }

        private void SaveCart(List<ShoppingCartItem> list)
        {
            HttpContext.Session.SetString("MyCartSession", JsonSerializer.Serialize(list));
        }
    }

    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public string TenDongHo { get; set; } = string.Empty;
        public decimal GiaBan { get; set; }
        public string HinhAnh { get; set; } = string.Empty;
        int _soLuong;
        public int SoLuong
        {
            get => _soLuong;
            set => _soLuong = value;
        }
    }
}