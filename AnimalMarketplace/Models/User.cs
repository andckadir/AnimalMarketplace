using AnimalMarketplace.Database.DTO;
using Newtonsoft.Json;

namespace AnimalMarketplace.Models;

public class User
{
    public User()
    {
        
    }
    
    public User(UserCreateDto userCreateDTO)
    {
        this.Name = userCreateDTO.Name;
        this.Surname = userCreateDTO.Surname;
        this.Email = userCreateDTO.Email;
        this.Phone = userCreateDTO.Phone;
        this.Gender = userCreateDTO.Gender;
        this.IsAdmin = false;
        this.Favorites = new List<Favorite>();
    }
    
    //User Properties
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateTime RegisterDate { get; set; }
    public string Phone { get; set; }
    public Gender Gender { get; set; }
    public bool IsAdmin { get; set; }
    public string PasswordHash { get; set; }
    
    //Navigation Properties
    public ICollection<Favorite> Favorites { get; set; }
    public Seller Seller { get; set; }
}