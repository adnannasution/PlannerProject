

namespace Plan.Models
{
public class TabelDokumen
{
    public int Id { get; set; }
    public int MemoId { get; set; }
    public string NamaDokumen { get; set; }  // Nama dokumen yang diberikan pengguna
    public string NamaFile { get; set; }  // Nama file yang diupload
    public string Path { get; set; }

   public Memo Memo { get; set; }
    // public Termin Termin { get; set; }
}


}