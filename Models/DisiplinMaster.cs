using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class DisiplinMaster
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Disiplin { get; set; }
    }
}
