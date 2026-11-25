using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using WebBanDongHo.Data;
using WebBanDongHo.Models;
using System;
using System.Threading.Tasks;

namespace WebBanDongHo.Pages
{
    [IgnoreAntiforgeryToken]
    public class ChatModel : PageModel
    {
        private readonly AppDbContext _context;
        public ChatModel(AppDbContext context) => _context = context;

        [BindProperty(SupportsGet = true)]
        public string Username { get; set; } = string.Empty;

        public void OnGet() { }

        public async Task<JsonResult> OnGetMessagesAsync(string username)
        {
            var msgs = await _context.TinNhans
                .Where(m => (m.NguoiGui == username && m.NguoiNhan == "admin") ||
                            (m.NguoiGui == "admin" && m.NguoiNhan == username))
                .OrderBy(m => m.ThoiGian)
                .Select(m => new { sender = m.NguoiGui, message = m.NoiDung })
                .ToListAsync();
            return new JsonResult(msgs);
        }

        public async Task<JsonResult> OnPostSendAsync([FromBody] ChatDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                dto.Username = Username;

            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Message))
                return new JsonResult(new { success = false, error = "Dữ liệu không đầy đủ" });

            var tin = new TinNhan
            {
                NguoiGui = dto.Username,
                NguoiNhan = "admin",
                NoiDung = dto.Message,
                ThoiGian = DateTime.UtcNow,
                DaDoc = false
            };
            _context.TinNhans.Add(tin);
            await _context.SaveChangesAsync();

            var hub = (IHubContext<WebBanDongHo.Hubs.ChatHub>)
                HttpContext.RequestServices.GetService(typeof(IHubContext<WebBanDongHo.Hubs.ChatHub>));
            if (hub != null)
                await hub.Clients.Group("admin").SendAsync("ReceiveMessage", dto.Username, dto.Message);

            return new JsonResult(new { success = true });
        }

        public class ChatDto
        {
            public string Username { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }
    }
}
