using AnimalMarketplace.Database.Dto;
using AnimalMarketplace.Models;

namespace AnimalMarketplace.Services;

public interface IUserService
{
    Task<ServiceResult<User>> UpdateUserAsync(int userId, UserUpdateDto dto, CancellationToken cancellationToken);
    Task<ServiceResult<User>> GetUserByIdAsync(int userId, CancellationToken cancellationToken);
    Task<ServiceResult<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> UserExistByEmailAsync(string email, CancellationToken cancellationToken);
    Task<ServiceResult<User>> DeleteUserAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult<Favorite>> AddFavoriteAsync(int userId,int advertId ,CancellationToken cancellationToken = default);
    Task<ServiceResult<Favorite>> RemoveFavoriteAsync(int userId,int advertId, CancellationToken cancellationToken = default);
    Task<bool> FavoriteExistsByUserIdAndAdvertIdAsync(int userId, int advertId,
        CancellationToken cancellationToken = default);
    Task<ServiceResult<List<Favorite>>> GetAllFavoritesAsync(int userId, CancellationToken cancellationToken = default);
}