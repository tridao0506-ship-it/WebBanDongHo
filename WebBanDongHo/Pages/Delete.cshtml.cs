using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebBanDongHo.Data;
using WebBanDongHo.Models;

namespace WebBanDongHo.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly WebBanDongHo.Data.AppDbContext _context;

        public DeleteModel(WebBanDongHo.Data.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DongHo DongHo { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dongho = await _context.DongHos.FirstOrDefaultAsync(m => m.Id == id);

            if (dongho == null)
            {
                return NotFound();
            }
            else
            {
                DongHo = dongho;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dongho = await _context.DongHos.FindAsync(id);
            if (dongho != null)
            {
                DongHo = dongho;
                _context.DongHos.Remove(DongHo);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
