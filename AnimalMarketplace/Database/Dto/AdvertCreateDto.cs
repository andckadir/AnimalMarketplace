using AnimalMarketplace.Models;

namespace AnimalMarketplace.Database.DTO;

public class AdvertCreateDto
{
    public decimal Price { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    
    // Animal bilgileri
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public AnimalKind Kind { get; set; }
}