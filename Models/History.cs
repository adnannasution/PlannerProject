using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class History
    {
        public int Id { get; set; }
        public int Tahun { get; set; }
        public string Bulan { get; set; }
        public string Minggu { get; set; }
        public float Nominal { get; set; }
    }
}
