using AnimalMarketplace.Database.DTO.Response;
using AnimalMarketplace.Models;

namespace AnimalMarketplace.Database.Dto.Response;

public class AdvertSimpleDto
{
    public int AdvertId { get; set; }
    public AnimalKind AnimalKind { get; set; }
    public decimal Price { get; set; }
    public string BusinessName { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Address Address { get; set; }
    public AdvertImageDto Image { get; set; }
    
    public AdvertSimpleDto(Advert advert)
    {
        this.AdvertId = advert.Id;
        this.AnimalKind = advert.Animal.Kind;
        this.Price = advert.Price;
        this.BusinessName = advert.Seller.BusinessName;
        this.Description = advert.Description;
        this.Title = advert.Title;
        this.Address = advert.Address;
        this.Image = new AdvertImageDto(advert.Images.FirstOrDefault(s => s.IsPrimary));
    }
}