using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class FaseMonitoring
    {
       
        public int Id { get; set; }

    [Required]
        public string Kode_Project { get; set; }

   
        public string Status_LKP { get; set; }

      
        public string Status_Volume { get; set; }

        public int? Count_Days_Progress { get; set; }

        public int? Count_Days_Over { get; set; }

     
        public string Status_Durasi { get; set; }

    
        public string Status_Kontrak { get; set; }

        public int? Amandemen_Nilai { get; set; }

        public decimal? Total_Nilai_Kontrak { get; set; }

        public decimal? Nilai_Tagihan { get; set; }
    }
}
