using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class WbsData
    {
       
        public int Id { get; set; }

   
        public string Bulan { get; set; }

       
        public int Tahun { get; set; }

        public decimal ActualWbsRt { get; set; }

       
        public decimal PercentActual { get; set; }

        public decimal CommitmentWbsRt { get; set; }

      
        public decimal PercentCommitment { get; set; }

        public decimal RopWbsRt { get; set; }

    
        public decimal PercentRop { get; set; }

     
        public decimal PercentForecasting { get; set; }

    
        public string JenisBiaya { get; set; }
    }
}
