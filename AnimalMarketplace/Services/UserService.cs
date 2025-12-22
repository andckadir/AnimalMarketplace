using AnimalMarketplace.Database.Dto;
using AnimalMarketplace.Models;
using AnimalMarketplace.Repositories.Implementations;
using AnimalMarketplace.Repositories.Interfaces;

namespace AnimalMarketplace.Services;

public class UserService(IUserRepository userRepository, 
    ISellerRepository sellerRepository,IAdvertRepository advertRepository,ITokenService tokenService) : IUserService
{
    public async Task<ServiceResult<User>> UpdateUserAsync(int userId, UserUpdateDto dto, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return ServiceResult<User>.Failure("User not found.");
        }
        
        if (user.Email != dto.Email)
        {
            bool emailExists = await userRepository.UserExistsByEmailAsync(dto.Email, cancellationToken);
            if (emailExists)
            {
                return ServiceResult<User>.Failure("This email address is already in use by another account.");
            }
        }
        
        user.Name = dto.Name;
        user.Surname = dto.Surname;
        user.Email = dto.Email;
        user.Phone = dto.Phone;
        user.Gender = dto.Gender;
        
        var updatedUser = await userRepository.UpdateUserAsync(user, cancellationToken);
        var token = tokenService.CreateToken(updatedUser!);

        return ServiceResult<User>.Success(token, updatedUser!);
    }

    public async Task<ServiceResult<User>> GetUserByIdAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);
        return user == null ? ServiceResult<User>.Failure("User not found.") : ServiceResult<User>.Success(null, user);
    }

    public async Task<ServiceResult<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByEmailAsync(email,cancellationToken);
        return user == null ? ServiceResult<User>.Failure("User not found.") : ServiceResult<User>.Success(null, user);
    }

    public async Task<bool> UserExistByEmailAsync(string email,CancellationToken cancellationToken)
    {
        return await userRepository.UserExistsByEmailAsync(email, cancellationToken);
    }

    public async Task<ServiceResult<User>> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetUserByIdAsync(id, cancellationToken);
        if (user == null)
            return ServiceResult<User>.Failure("User not found.");
            
        await userRepository.DeleteUserAsync(user, cancellationToken);
        return ServiceResult<User>.Success(null!, user);
    }

    public async Task<ServiceResult<Favorite>> AddFavoriteAsync(int userId, int advertId, CancellationToken cancellationToken = default)
    {
        var userExist = await userRepository.UserExistsByIdAsync(userId, cancellationToken);
        if (!userExist)
            return ServiceResult<Favorite>.Failure("User not found.");
        var advertExist = await advertRepository.AdvertExistById(advertId, cancellationToken);
        if (!advertExist)
            return ServiceResult<Favorite>.Failure("Advert not found.");
        var favoriteExist = await userRepository.FavoriteExistsByUserIdAndAdvertIdAsync(userId, advertId, cancellationToken);
        if (favoriteExist)
            return ServiceResult<Favorite>.Failure("Favorite already exists.");
        var favorite = await userRepository.AddFavoriteAsync(new Favorite{AdvertId =  advertId, UserId = userId}, cancellationToken);
        return ServiceResult<Favorite>.Success(null!,favorite);
    }

    public async Task<ServiceResult<Favorite>> RemoveFavoriteAsync(int userId, int advertId, CancellationToken cancellationToken = default)
    {
        var userExist = await userRepository.UserExistsByIdAsync(userId, cancellationToken);
        if (!userExist)
            return ServiceResult<Favorite>.Failure("User not found.");
        var advertExist = await advertRepository.AdvertExistById(advertId, cancellationToken);
        if (!advertExist)
            return ServiceResult<Favorite>.Failure("Advert not found.");
        var favoriteExist = await userRepository.FavoriteExistsByUserIdAndAdvertIdAsync(userId, advertId, cancellationToken);
        if (!favoriteExist)
            return ServiceResult<Favorite>.Failure("Favorite not exists.");
        var favorite = await userRepository.RemoveFavoriteAsync(new Favorite{AdvertId =  advertId, UserId = userId}, cancellationToken);
        return ServiceResult<Favorite>.Success(null!,favorite);
    }

    public async Task<bool> FavoriteExistsByUserIdAndAdvertIdAsync(int userId, int advertId, CancellationToken cancellationToken = default)
    {
        return await userRepository.FavoriteExistsByUserIdAndAdvertIdAsync(userId, advertId, cancellationToken);
    }

    public async Task<ServiceResult<List<Favorite>>> GetAllFavoritesAsync(int userId, CancellationToken cancellationToken = default)
    {
        var userExist = await userRepository.UserExistsByIdAsync(userId, cancellationToken);
        if (!userExist)
            return ServiceResult<List<Favorite>>.Failure("User not found.");
        var favorites = await userRepository.GetAllFavoritesAsync(userId, cancellationToken);
            return ServiceResult<List<Favorite>>.Success(null!, favorites);
    }
}