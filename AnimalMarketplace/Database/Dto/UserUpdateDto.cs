using AnimalMarketplace.Models;

namespace AnimalMarketplace.Database.Dto;

public class UserUpdateDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public Gender Gender { get; set; }
}