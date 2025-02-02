using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class KategoriEquipmentMaster
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Kategori { get; set; }
    }
}
