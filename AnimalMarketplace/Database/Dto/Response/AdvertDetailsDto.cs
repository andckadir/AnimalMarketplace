using AnimalMarketplace.Models;

namespace AnimalMarketplace.Database.DTO.Response;

public class AdvertDetailsDto
{
    public int AdvertId { get; set; }
    public string SellerName { get; set; }
    public string SellerSurname { get; set; }
    public string SellerPhone { get; set; }
    public string SellerBusinessName { get; set; }
    public Gender AnimalGender { get; set; }
    public int AnimalAge { get; set; }
    public AnimalKind AnimalKind { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
    public Address Address { get; set; }
    public DateTime Date { get; set; }
    public AdvertState State { get; set; }
    public List<AdvertImageDto> Images { get; set; }
    
    public AdvertDetailsDto(Advert advert)
    {
        this.AdvertId = advert.Id;
        this.SellerName = advert.Seller.User.Name;
        this.SellerSurname = advert.Seller.User.Surname;
        this.SellerPhone = advert.Seller.User.Phone;
        this.SellerBusinessName = advert.Seller.BusinessName;
        this.AnimalGender = advert.Animal.Gender;
        this.AnimalAge = advert.Animal.Age;
        this.AnimalKind = advert.Animal.Kind;
        this.Price = advert.Price;
        this.Description = advert.Description;
        this.Title = advert.Title;
        this.Address = advert.Address;
        this.Date = advert.Date;
        this.State = advert.State;
        this.Images = advert.Images?.Select(img => new AdvertImageDto(img)).ToList();
    }
}

