namespace WebBanDongHo.Models 
{
    public class DongHo
    {
        public int Id { get; set; }
        public string TenDongHo { get; set; } = string.Empty;
        public decimal GiaBan { get; set; }
        public string HinhAnh { get; set; } = string.Empty;

        public int SoLuongTon { get; set; }
    }
}