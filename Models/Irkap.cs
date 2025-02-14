using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class Irkap
    {
        [Key]
        public int Id { get; set; }
        public string NoProgram { get; set; }
        public string Judul { get; set; }
        public string Disiplin { get; set; }
    }
}
