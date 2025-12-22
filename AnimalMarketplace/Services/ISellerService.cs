using AnimalMarketplace.Database.Dto;
using AnimalMarketplace.Models;

namespace AnimalMarketplace.Services;

public interface ISellerService
{
    Task<ServiceResult<Seller>> GetSellerByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<Seller>> CreateSellerAsync(int userId,CreateSellerDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult<Seller>> UpdateSellerAsync(int userId,CreateSellerDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult<Seller>> DeleteSellerAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> SellerExistByUserIdAsync(int sellerId, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> GetSellerIdByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}