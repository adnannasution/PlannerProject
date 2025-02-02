using System;
using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class Timeline
    {
       
        public int Id { get; set; }

        public string Kode_Project { get; set; }

        public DateTime? Memo_turun { get; set; }
        public DateTime? Menyusun_GTC_dan_RAB { get; set; }
        public DateTime? Review_GTC_RAB { get; set; }
        public DateTime? Menyiapkan_dan_mengirim_memo_comment_bestek { get; set; }
        public DateTime? Revisi_GTC_dan_RAB_sesuai_comment_bestek { get; set; }
        public DateTime? Sourcing_penawaran_ke_vendor { get; set; }
        public DateTime? Membuat_analisa_cost_estimasi { get; set; }
        public DateTime? Review_analisa_cost_estimasi { get; set; }
        public DateTime? Menyiapkan_RKS { get; set; }
        public DateTime? Menyiapkan_RAB { get; set; }
        public DateTime? Membuat_analisa_TKDN { get; set; }
        public DateTime? Membuat_analisa_RAM { get; set; }
        public DateTime? Membuat_ijin_persetujuan_prinsip { get; set; }
        public DateTime? Jobplan_Ext_mengajukan_release_dan_print_PR { get; set; }
        public DateTime? Membuat_checklist_kelengkapan_DP3 { get; set; }
        public DateTime? Menyiapkan_memo_pengantar_DP3_Paket { get; set; }
        public DateTime? Mengirim_dokumen_DP3_Paket_via_P_Office { get; set; }
    }
}
