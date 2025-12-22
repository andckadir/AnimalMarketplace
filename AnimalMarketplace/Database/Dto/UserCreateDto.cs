using AnimalMarketplace.Models;

namespace AnimalMarketplace.Database.DTO;

public class UserCreateDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public Gender Gender { get; set; }
    public string Password { get; set; }
}