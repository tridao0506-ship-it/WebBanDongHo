using System.ComponentModel.DataAnnotations;

namespace WebBanDongHo.Models
{
    public class NguoiDung
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string HoTen { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? VaiTro { get; set; }
    }
}