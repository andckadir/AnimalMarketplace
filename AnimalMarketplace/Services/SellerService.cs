using AnimalMarketplace.Database.Dto;
using AnimalMarketplace.Models;
using AnimalMarketplace.Repositories.Interfaces;

namespace AnimalMarketplace.Services;

public class SellerService(ISellerRepository sellerRepository,IUserRepository userRepository) : ISellerService
{
    
    public async Task<ServiceResult<Seller>> GetSellerByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var seller = await sellerRepository.GetSellerByUserIdAsync(userId, cancellationToken);
        return seller == null ? ServiceResult<Seller>.Failure("Seller not found") : ServiceResult<Seller>.Success(null!, seller);
    }

    public async Task<ServiceResult<Seller>> CreateSellerAsync(int userId, CreateSellerDto dto, CancellationToken cancellationToken = default)
    {
        var userExists = await userRepository.UserExistsByIdAsync(userId, cancellationToken);
        if (!userExists)
            return ServiceResult<Seller>.Failure("Seller can not be created. User not found.");
        
        var sellerExists = await sellerRepository.SellerExistByUserIdAsync(userId, cancellationToken);
        if (sellerExists)
            return ServiceResult<Seller>.Failure("Seller already exists.");
        
        var seller = await sellerRepository.AddSellerAsync(new Seller(){UserId = userId,BusinessName = dto.BusinessName}, cancellationToken);
        return seller == null ? ServiceResult<Seller>.Failure("Seller creation failed at database level.") : ServiceResult<Seller>.Success(null!, seller);
    }

    public async Task<bool> SellerExistByUserIdAsync(int sellerId, CancellationToken cancellationToken = default)
    {
        return await sellerRepository.SellerExistByUserIdAsync(sellerId, cancellationToken);
    }

    public async Task<ServiceResult<int>> GetSellerIdByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var sellerId = await sellerRepository.GetSellerIdByUserIdAsync(userId, cancellationToken);
        return sellerId == null ? ServiceResult<int>.Failure("User is not a seller") : ServiceResult<int>.Success(null!, sellerId.Value);  
    }

    public async Task<ServiceResult<Seller>> UpdateSellerAsync(int sellerId, CreateSellerDto dto, CancellationToken cancellationToken = default)
    {
        var seller =  await sellerRepository.GetSellerByUserIdAsync(sellerId, cancellationToken);
        if (seller == null)
            return ServiceResult<Seller>.Failure("Seller not found");
        seller.BusinessName = dto.BusinessName;
        await sellerRepository.UpdateSellerAsync(seller, cancellationToken);
        return ServiceResult<Seller>.Success(null!,seller);
    }

    public async Task<ServiceResult<Seller>> DeleteSellerAsync(int userId, CancellationToken cancellationToken = default)
    {   
        var seller = await sellerRepository.GetSellerByUserIdAsync(userId, cancellationToken);
        if (seller == null)
            return ServiceResult<Seller>.Failure("Seller not found");
        await sellerRepository.DeleteSellerAsync(seller, cancellationToken);
        return ServiceResult<Seller>.Success(null!, seller);
    }
}