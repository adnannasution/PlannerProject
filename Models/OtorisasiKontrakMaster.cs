using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class OtorisasiKontrakMaster
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)] // Sesuaikan panjang maksimum jika diketahui
        public string Otorisasi { get; set; }
    }
}
