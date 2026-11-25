using System.ComponentModel.DataAnnotations;

namespace WebBanDongHo.Models
{
    public class DonHang
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        public string DiaChi { get; set; } = string.Empty;

        public DateTime NgayDat { get; set; } = DateTime.Now;
        public decimal TongTien { get; set; }

        public string TrangThai { get; set; } = "Đang xử lý";

        public List<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
        public string? TenKhachHang { get; internal set; }
        public string? NguoiDungId { get; internal set; }
    }
}