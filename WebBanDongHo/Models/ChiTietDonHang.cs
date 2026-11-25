namespace WebBanDongHo.Models
{
    public class ChiTietDonHang
    {
        public int Id { get; set; }
        public int DonHangId { get; set; }
        public DonHang? DonHang { get; set; }

        public int DongHoId { get; set; }
        public string TenDongHo { get; set; } = string.Empty;
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
    }
}