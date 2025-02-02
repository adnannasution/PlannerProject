using System.ComponentModel.DataAnnotations.Schema;

namespace Plan.Models
{

public class Termin
{
    public int Id { get; set; }

  
    public string Kode_Project { get; set; }
    public int JenisTermin { get; set; }
    public string Evaluasi_Planner { get; set; }
    public string No_WO_Tagihan { get; set; }
  
    public decimal Prosentase_Tagihan { get; set; }
    public string SA { get; set; }
    public DateTime Periode { get; set; }
    public DateTime Periode2 { get; set; }
    public decimal Tagihan { get; set; }


    public string Dokumen { get; set; }
    public decimal Nilai_Plan { get; set; }

    public decimal Nilai_Tagihan { get; set; }  

    


   // public FaseTender FaseTender { get; set; }
}

}
