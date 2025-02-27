namespace Plan.Models
{
    public class HistoryTermin
    {
        public int Id { get; set; }
        public string Kode_Project { get; set; }
        public string NamaDokumen { get; set; }
        public string Aksi { get; set; }
        public DateTime Tanggal { get; set; }
        public TimeSpan? Waktu { get; set; } // Nullable Time field
     
        public string Email { get; set; }
    }
}
