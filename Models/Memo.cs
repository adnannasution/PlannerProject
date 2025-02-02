using System;

namespace Plan.Models
{
    public class Memo
    {
        public int Id { get; set; }
        public string No_Memo_Rekomendasi { get; set; }
        public DateTime Tanggal_Masuk_Memo { get; set; }
        public string No_Memo_Kirim_Paket { get; set; }
        public string Dokumen { get; set; } // Stores the binary data of the document
        public string Email { get; set; } // Nullable string
        public string KebutuhanKontrak { get; set; } // Nullable string
        public string Judul { get; set; } // Nullable string
        public string Disiplin { get; set; } // Nullable string
    }
}
 