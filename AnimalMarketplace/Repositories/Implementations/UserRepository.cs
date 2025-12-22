using AnimalMarketplace.Database.DbContexts;
using AnimalMarketplace.Models;
using AnimalMarketplace.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AnimalMarketplace.Repositories.Implementations;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await context.UserDbSet
            .Include(u => u.Favorites) 
            .ToListAsync(cancellationToken);
        return users;
    }//
    
    public async Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.UserDbSet.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }//
    
    public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await context.UserDbSet
            .Include(u => u.Favorites)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        return user;
    }//
    
    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.UserDbSet
            .Include(u => u.Favorites)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }//
    
    public async Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.UserDbSet
            .AnyAsync(u => u.Email == email, cancellationToken);
    }//

    public async Task<bool> UserExistsByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await context.UserDbSet
            .AnyAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<Favorite> AddFavoriteAsync(Favorite favorite, CancellationToken cancellationToken = default)
    {
        context.FavoriteDbSet.Add(favorite);
        await context.SaveChangesAsync(cancellationToken);
        return favorite;
    }

    public async Task<Favorite> RemoveFavoriteAsync(Favorite favorite, CancellationToken cancellationToken = default)
    {
        context.FavoriteDbSet.Remove(favorite);
        await context.SaveChangesAsync(cancellationToken);
        return favorite;
    }

    public async Task<bool> FavoriteExistsByUserIdAndAdvertIdAsync(int userId,int advertId, CancellationToken cancellationToken = default)
    {
        return await context.FavoriteDbSet.AnyAsync(f=>f.AdvertId == advertId  && f.UserId == userId, cancellationToken);
    }

    public async Task<List<Favorite>> GetAllFavoritesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await context.FavoriteDbSet.Where(f => f.UserId == userId).OrderBy(f => f.AdvertId).ToListAsync(cancellationToken);
    }
    //

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        context.UserDbSet.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }//
    
    public async Task<User> DeleteUserAsync(User user, CancellationToken cancellationToken = default)
    {
        context.UserDbSet.Remove(user);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }//
}