using System.Text.Json.Serialization;
using AnimalMarketplace.Database.DTO;
using AnimalMarketplace.Models;

namespace AnimalMarketplace.Models;

public class Advert
{
    public Advert()
    {
        
    }
    
    public Advert(int sellerId,AdvertCreateDto dto)
    {
        Price = dto.Price;
        Description = dto.Description;
        Title = dto.Title;
        Address = new Address
        {
            City = dto.City,
            District = dto.District
        };
        Animal = new Animal(dto);
        Date = DateTime.UtcNow;
        State = AdvertState.Active;
        SellerId = sellerId;
    }
    
    //Advert Properties
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public AdvertState State { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
    
    //Foreign Key
    public int SellerId { get; set; }
    
    //Owned Property
    public Address Address { get; set; }
    
    //Navigation Properties
    public Animal Animal { get; set; }
    public Seller Seller { get; set; }
    public ICollection<Favorite> Favorites { get; set; }
    public ICollection<AdvertImage> Images { get; set; }
}