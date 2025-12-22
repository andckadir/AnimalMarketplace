using AnimalMarketplace.Database.DbContexts;
using AnimalMarketplace.Models;
using AnimalMarketplace.Repositories.Interfaces;
using AnimalMarketplace.Services;
using Microsoft.EntityFrameworkCore;

namespace AnimalMarketplace.Repositories.Implementations;

public class SellerRepository(ApplicationDbContext context, IUserRepository userRepository) : ISellerRepository
{
    public async Task<List<Seller>> GetAllSellersAsync(CancellationToken cancellationToken = default)
    {
        var sellers = await context.SellerDbSet
            .Include(s => s.Adverts) 
            .ToListAsync(cancellationToken);
        return sellers;
    }//
    public async Task<Seller?> GetSellerByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var seller = await context.SellerDbSet
            .Include(s => s.Adverts)
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
        return seller;
    } //
    public async Task<Seller> AddSellerAsync(Seller seller, CancellationToken cancellationToken = default)
    {
        await context.SellerDbSet.AddAsync(seller, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return seller;
    } //
    public async Task<bool> SellerExistByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await context.SellerDbSet.AnyAsync(s => s.UserId == userId, cancellationToken);
    } //
    public async Task<Seller> UpdateSellerAsync(Seller seller, CancellationToken cancellationToken = default)
    {
        context.SellerDbSet.Update(seller);
        await context.SaveChangesAsync(cancellationToken);
        return seller;
    }//
    public async Task<Seller> DeleteSellerAsync(Seller seller, CancellationToken cancellationToken = default)
    {
        context.SellerDbSet.Remove(seller);
        await context.SaveChangesAsync(cancellationToken);
        return seller;
    }//

    public async Task<int?> GetSellerIdByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var seller = await GetSellerByUserIdAsync(userId, cancellationToken);
        return seller?.Id;
    }
}