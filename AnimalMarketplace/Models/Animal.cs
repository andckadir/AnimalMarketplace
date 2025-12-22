using AnimalMarketplace.Database.DTO;
using AnimalMarketplace.Models;
using Newtonsoft.Json;

namespace AnimalMarketplace.Models;

public class Animal
{
    public Animal()
    {
        
    }
    
    public Animal(AdvertCreateDto dto)
    {
        Gender = dto.Gender;
        Age = dto.Age;
        Kind = dto.Kind;
    }
    
    //Animal Properties
    public int Id { get; set; }
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public AnimalKind Kind { get; set; }
    
    //Foreign Keys
    public int AdvertId { get; set; }
    
    // Navigation Properties
    public Advert Advert { get; set; }
}