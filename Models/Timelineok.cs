using System;
using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class Timelineok
    {
        public int Id { get; set; }

       [Required]
        public string Kode_Project { get; set; }

 
        public string Task { get; set; }
        public string Pic { get; set; }

  
        public DateTime Tanggal { get; set; }
        public DateTime? Target { get; set; }

   
        public string Dokumen { get; set; }
        public string ResumeStatusPekerjaan { get; set; }

          public string Status { get; set; }
          public string Keterangan { get; set; }

    }
}
