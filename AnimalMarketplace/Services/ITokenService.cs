using AnimalMarketplace.Models;

namespace AnimalMarketplace.Services;

public interface ITokenService
{
    string CreateToken(User user);
}