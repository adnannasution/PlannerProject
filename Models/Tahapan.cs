namespace Plan.Models
{
    public class Tahapan
    {
        public int Id { get; set; }
        public DateTime? Tanggal { get; set; } // Nullable Date field
        public TimeSpan? Waktu { get; set; } // Nullable Time field
        public string Email { get; set; } // Nullable Email field
        public string Tahap { get; set; } // Nullable Tahap field
        public string Kode_Project { get; set; } // Nullable Tahap field
        public string No_Memo_Rekomendasi { get; set; } // Nullable Tahap field
        public string Status { get; set; } // Nullable Tahap field
        public string Keterangan { get; set; } // Nullable Tahap field
    }
}
