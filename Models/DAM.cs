using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class DAM
    {
        public int Id { get; set; }

        [StringLength(250)]
        public string Area { get; set; }

        [StringLength(250)]
        public string Minggu { get; set; }

        [StringLength(150)]
        public string Bulan { get; set; }

        public int Tahun { get; set; }
        public string Tagno { get; set; }
        public string Problem { get; set; }
        public string Pic { get; set; }
        public string Status { get; set; }
        public string PsAction { get; set; }
    }
}
