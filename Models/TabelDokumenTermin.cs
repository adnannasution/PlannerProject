namespace Plan.Models
{
public class TabelDokumenTermin
{
    public int Id { get; set; }
    public string Kode_Project { get; set; }
    public string NamaDokumen { get; set; }  // Nama dokumen yang diberikan pengguna
    public string NamaFile { get; set; }  // Nama file yang diupload
    public string Path { get; set; }
    public int JenisTermin { get; set; }

 
 
}


}