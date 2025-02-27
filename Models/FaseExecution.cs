using System;

namespace Plan.Models
{
    public class FaseExecution
    {
        public int Id { get; set; }

      
        public string Kode_Project { get; set; }

        public DateTime? Start_Date { get; set; }
        public DateTime End_Date_MPL { get; set; }
        public int? Amandemen_Waktu { get; set; }
        public DateTime End_Date { get; set; }
        public int? Durasi_Kontrak { get; set; }
        public int? Durasi_Aktual_HK { get; set; }
        public DateTime Date_LKP { get; set; }
        public decimal? Plan_Progress_Fisik { get; set; }
        public decimal? Progress_Fisik_0 { get; set; }
        public decimal? Progress_Fisik { get; set; }
        public string Status_Performance { get; set; }
        public decimal? Progress_Finansial { get; set; }
        public int? LKP_Time_Limit { get; set; }
    }
}
