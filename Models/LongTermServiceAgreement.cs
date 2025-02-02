using System;

namespace Plan.Models
{
    public class LongTermServiceAgreement
    {
        public int Id { get; set; }  // Primary key
        public string No { get; set; }  // Serial number or identification number
        public string Pekerjaan { get; set; }  // Description of the work
        public string SpNo { get; set; }  // SP Number
        public string PoNo { get; set; }  // PO Number
        public DateTime MulaiPekerjaan { get; set; }  // Start date of the work
        public DateTime SelesaiPekerjaan { get; set; }  // End date of the work
        public string Pemborong { get; set; } 
        
        public string Uraian { get; set; }  // Contractor's name
         // Contractor's name
        public decimal NilaiKontrak { get; set; }  // Contract value
        public decimal NilaiKontrak_Persen { get; set; }  // Contract value
        public decimal TagihanSebelumnya { get; set; }  // Previous invoice
        public decimal TagihanSebelumnya_Persen { get; set; }  // Previous invoice
        public decimal TagihanII { get; set; }  // Invoice II
        public decimal TagihanII_Persen { get; set; }  // Invoice II
        public decimal Akumulasi { get; set; }  // Accumulated amount
        public decimal Akumulasi_Persen { get; set; }  // Accumulated amount
        public decimal SisaKontrak { get; set; }  // Remaining contract value
        public decimal SisaKontrak_Persen { get; set; }  // Remaining contract value
        public string Status { get; set; }  // Status of the contract
    }
}
