namespace Plan.Models
{
    public class TaskMaster
    {
        public int Id { get; set; }
        public string Task { get; set; }
        public int Sla { get; set; }
        public DateTime Target { get; set; }
         public string Kode_Project { get; set; }
    }
}
