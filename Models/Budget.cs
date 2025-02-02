using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class Budget
    {
        public int Id { get; set; }


        public string Bulan { get; set; }

     
        public int Tahun { get; set; }

     
        public string Minggu { get; set; }

      
        public decimal Nominal { get; set; }
    }
}
