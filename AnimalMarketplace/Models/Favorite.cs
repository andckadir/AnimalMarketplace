using AnimalMarketplace.Models;
using Newtonsoft.Json;

namespace AnimalMarketplace.Models;

public class Favorite
{
    //Foreign Keys
    public int UserId { get; set; }
    public int AdvertId { get; set; }
    
    //Navigation Properties
    public User User { get; set; }
    public Advert Advert { get; set; }
}