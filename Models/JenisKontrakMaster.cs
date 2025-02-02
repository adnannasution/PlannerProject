using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class JenisKontrakMaster
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Jenis { get; set; }
    }
}
