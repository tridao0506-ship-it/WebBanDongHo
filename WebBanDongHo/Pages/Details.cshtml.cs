using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBanDongHo.Data;
using WebBanDongHo.Models;

namespace WebBanDongHo.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public DongHo DongHo { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            if (id == null) return NotFound();

            DongHo = _context.DongHos.FirstOrDefault(m => m.Id == id);

            if (DongHo == null) return NotFound();

            return Page();
        }
    }
}