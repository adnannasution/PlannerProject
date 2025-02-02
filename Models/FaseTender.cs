using System;
using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class FaseTender
    {
        public int Id { get; set; }  // Primary Key

        [Required]
        public string Kode_Project { get; set; }  // Nomor Vendor
   
        public string No_Vendor { get; set; }  // Nomor Vendor

        public string Pelaksana { get; set; }  // Pelaksana Tender

        public string PO_OA { get; set; }  // Nilai PO OA
        public string PO_SR { get; set; }  // Nilai PO SR
        public string PO_RO { get; set; }  // Nilai PO RO

        public string PR_OA { get; set; }  // Nilai PR OA
        public string PR_MT { get; set; }  // Nilai PR MT
        public string PR_SR { get; set; }  // Nilai PR SR

        public string Nomor_SP3MK { get; set; }  // Nomor SP3MK


        public decimal? Nilai_Purchasing_Order { get; set; }  // Nilai Purchasing Order
        public decimal? Nilai_Purchasing_Request { get; set; }  // Nilai Purchasing Request

        public int? Durasi_Masa_Penyelesaian_MPL { get; set; }  // Durasi Penyelesaian MPL (misalnya dalam hari)

        public int? Bulan { get; set; } 
        public string Otorisasi { get; set; } 
        public string Notif { get; set; } 
        public string Buyer { get; set; } 
    }
}
