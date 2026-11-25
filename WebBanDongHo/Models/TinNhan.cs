using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDongHo.Models
{
    public class TinNhan
    {
        [Key]
        public int Id { get; set; }

        public string NguoiGui { get; set; } = string.Empty; 
        public string NguoiNhan { get; set; } = string.Empty; 

        [Required]
        public string NoiDung { get; set; } = string.Empty;

        public DateTime ThoiGian { get; set; } = DateTime.Now;

        public bool DaDoc { get; set; } = false;
    }
}