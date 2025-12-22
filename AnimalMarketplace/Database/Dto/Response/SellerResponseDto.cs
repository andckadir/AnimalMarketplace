using AnimalMarketplace.Models;

namespace AnimalMarketplace.Database.Dto.Response;

public class SellerResponseDto(Seller seller,User user)
{
    public string Name { get; set; } = user.Name;
    public string Surname { get; set; } = user.Surname;
    public string BusinessName { get; set; } = seller.BusinessName;
    public string Phone { get; set; } = user.Phone;
    public List<int> AdvertsIds { get; set; } = seller.Adverts.Select(advert => advert.Id).ToList();
}