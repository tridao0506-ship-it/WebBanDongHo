using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebBanDongHo.Data;
using WebBanDongHo.Models;

namespace WebBanDongHo.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<DongHo> DongHos { get; set; } = default!;
        public string CurrentFilter { get; set; } = string.Empty;
        public string CurrentSort { get; set; } = string.Empty;

        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        private const int PageSize = 8; 

        public async Task OnGetAsync(string searchString, string sortOrder, int? pageIndex)
        {
            CurrentFilter = searchString;
            CurrentSort = sortOrder;

            var products = from d in _context.DongHos
                           select d;

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.TenDongHo.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(s => s.GiaBan);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(s => s.GiaBan);
                    break;
                default:
                    products = products.OrderByDescending(s => s.Id);
                    break;
            }

            int count = await products.CountAsync();
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);
            if (TotalPages < 1) TotalPages = 1;

            PageIndex = pageIndex ?? 1;
            if (PageIndex > TotalPages) PageIndex = TotalPages;
            if (PageIndex < 1) PageIndex = 1;

            DongHos = await products
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetClientMessagesAsync()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return new JsonResult(new List<object>());

            string username = User.Identity.Name ?? "";

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

        public async Task<IActionResult> OnGetClientSendMsgAsync(string message)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(message))
            {
                return new JsonResult(new { success = false });
            }

            string username = User.Identity.Name ?? "";

            var tinMoi = new TinNhan
            {
                NguoiGui = username,
                NguoiNhan = "admin",
                NoiDung = message.Trim(),
                ThoiGian = DateTime.Now,
                DaDoc = false
            };

            _context.TinNhans.Add(tinMoi);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }
}