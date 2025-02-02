using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class StatusKontrakMaster
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
