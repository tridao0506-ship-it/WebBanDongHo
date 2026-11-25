using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebBanDongHo.Data;
using WebBanDongHo.Models;

namespace WebBanDongHo.Pages.Admin
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public DongHo DongHo { get; set; } = default!;

        [BindProperty]
        public IFormFile? ImageFile { get; set; } // Bi?n nh?n file ?nh t? form

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ImageFile != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string uploadFolder = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.Exists(uploadFolder);
                }

                string filePath = Path.Combine(uploadFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                DongHo.HinhAnh = fileName;
            }
            else
            {
                DongHo.HinhAnh = "applewatch.jpg"; 
            }

            _context.DongHos.Add(DongHo);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}