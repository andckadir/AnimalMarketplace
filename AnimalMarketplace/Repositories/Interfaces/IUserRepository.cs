using AnimalMarketplace.Models;

namespace AnimalMarketplace.Repositories.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User> DeleteUserAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> UserExistsByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Favorite> AddFavoriteAsync(Favorite favorite, CancellationToken cancellationToken = default);
    Task<Favorite> RemoveFavoriteAsync(Favorite favorite, CancellationToken cancellationToken = default);

    Task<bool> FavoriteExistsByUserIdAndAdvertIdAsync(int userId, int advertId,
        CancellationToken cancellationToken = default);
    
    Task<List<Favorite>> GetAllFavoritesAsync(int userId, CancellationToken cancellationToken = default);
}