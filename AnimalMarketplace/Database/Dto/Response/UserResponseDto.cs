using AnimalMarketplace.Models;

namespace AnimalMarketplace.Database.Dto.Response;

public class UserResponseDto(User user)
{
    public string Name { get; set; } = user.Name;
    public string Surname { get; set; } =  user.Surname;
    public string Email { get; set; } = user.Email;
    public string Phone { get; set; } =  user.Phone;
    public Gender Gender { get; set; } = user.Gender;
    public List<int> FavoritesAdvertIds { get; set; } = user.Favorites.Select(f=>f.AdvertId).ToList();
    public DateTime Date { get; set; } =  user.RegisterDate;
}