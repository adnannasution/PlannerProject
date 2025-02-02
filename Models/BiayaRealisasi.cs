namespace Plan.Models
{
public class BiayaRealisasi
{
    public int Id { get; set; }
    public string JenisBiaya { get; set; }
    public string CI { get; set; }
    public string ApprRKPL { get; set; }
    public decimal RealisasiAssign { get; set; }
    public decimal PersenRealisasiAssign { get; set; }
    public decimal RealisasiActual { get; set; }
    public decimal PersenRealisasiActual { get; set; }
    public decimal RealisasiCommit { get; set; }
    public decimal PersenRealisasiCommit { get; set; }
    public decimal RealisasiROP { get; set; }
    public decimal PersenRealisasiROP { get; set; }
    public string Bulan { get; set; }
    public int Tahun { get; set; }
    public string Minggu { get; set; }
    public string Tipe_Project { get; set; }
    public string OH { get; set; }
}

}
