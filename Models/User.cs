using System.ComponentModel.DataAnnotations;

namespace Plan.Models
{
    public class User
    {
        public int Id { get; set; }

    
        public string Nama { get; set; }

       
        public string Fungsi { get; set; }

      
        public string Jabatan { get; set; }

     
        public string Email { get; set; }

        public string Pass { get; set; }
        public string RetypePass { get; set; }

        
         public string Rule { get; set; }
         public string Disiplin { get; set; }
    }
}
