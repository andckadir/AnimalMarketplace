using AnimalMarketplace.Models;

namespace AnimalMarketplace.Repositories.Interfaces;

public interface ISellerRepository
{
    
    Task<List<Seller>> GetAllSellersAsync(CancellationToken cancellationToken = default);
    Task<Seller?> GetSellerByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Seller> AddSellerAsync(Seller seller, CancellationToken cancellationToken = default);
    Task<Seller> UpdateSellerAsync(Seller seller, CancellationToken cancellationToken = default);
    Task<Seller> DeleteSellerAsync(Seller seller, CancellationToken cancellationToken = default);
    Task<bool> SellerExistByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<int?> GetSellerIdByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}