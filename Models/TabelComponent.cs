namespace Plan.Models
{

public class TabelComponent
{
 
    public int Id { get; set; }

 
    public string Kode_Project { get; set; }

    public string Component { get; set; }


    public string Description { get; set; }

 
    public int Quantity { get; set; }


    public string Unit { get; set; }


    public decimal ValuationPrice { get; set; }


    public DateTime DeliveryDate { get; set; }
}

}
