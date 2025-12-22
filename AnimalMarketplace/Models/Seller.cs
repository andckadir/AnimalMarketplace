using System.Text.Json.Serialization;
using AnimalMarketplace.Models;

namespace AnimalMarketplace.Models;

public class Seller
{
    public Seller()
    {
        
    }

    public int Id { get; set; }
    
    //Foreign Key
    public int UserId { get; set; }
    
    //Seller Properties
    public string BusinessName { get; set; }
    
    //Navigation Property
    public User User { get; set; }
    public ICollection<Advert> Adverts  { get; set; }
}