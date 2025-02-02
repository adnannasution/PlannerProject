using System;
using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class FasePlanning
    {
        public int Id { get; set; }

        public string Kode_Project { get; set; }

        public int No { get; set; }


    


        public string Disiplin { get; set; }


        public string Pekerjaan { get; set; }

        public string Kategori_Equipment { get; set; }


        public string Kategori_Maintenance { get; set; }


        public string Area { get; set; }


        public string Status_kontrak { get; set; }
        public string Kategori_Kontrak { get; set; }

        public string Direksi { get; set; }


        public string Jenis_Kontrak { get; set; }


        public string Planner { get; set; }


        public string No_Memo_Rekomendasi { get; set; }


        public DateTime? Tanggal_Masuk_Memo { get; set; }


        public string No_Memo_Kirim_Paket { get; set; }


        public DateTime? Tanggal_Kirim_Paket { get; set; }


        public string Status_Next_Contract { get; set; }

        public string Informasi_Detail { get; set; }

        public string Nomor_PR_Kontrak_Baru { get; set; }

        public DateTime? Tanggal_Update { get; set; }

        public int Tahun { get; set; }
        public string Tipe_Project { get; set; }
        public string Jenis_Project { get; set; }
        public string NoProgram { get; set; }
        public string MOC { get; set; }
        public string Notifikasi { get; set; }
    }
}
