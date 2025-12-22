using AnimalMarketplace.Database.DbContexts;
using AnimalMarketplace.Database.DTO;
using AnimalMarketplace.Models;
using AnimalMarketplace.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AnimalMarketplace.Repositories.Implementations;

public class AdvertRepository(ApplicationDbContext context) : IAdvertRepository
{
    public async Task<Advert?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.AdvertDbSet
            .Include(a => a.Images.OrderBy(img => img.Order))
            .Include(a => a.Animal)
            .Include(a => a.Seller)
            .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Advert>> GetAllWithPrimaryImageAsync(CancellationToken cancellationToken = default)
    {
        return await context.AdvertDbSet
            .Include(a => a.Images.Where(img => img.IsPrimary))
            .Include(a => a.Animal)
            .Include(a => a.Seller)
            .ToListAsync(cancellationToken);
    }

    public async Task<Advert> CreateAsync(Advert advert, List<AdvertImage> images,
        CancellationToken cancellationToken = default)
    {
        advert.Images = images;
        await context.AdvertDbSet.AddAsync(advert, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return advert;
    }

    public async Task<Advert> UpdateAsync(Advert advert, CancellationToken cancellationToken = default)
    {
        context.AdvertDbSet.Update(advert);
        await context.SaveChangesAsync(cancellationToken);
        return advert;
    }

    public async Task<Advert?> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var advert = await GetByIdWithDetailsAsync(id, cancellationToken);
        if (advert == null) return null;
        context.AdvertDbSet.Remove(advert);
        await context.SaveChangesAsync(cancellationToken);
        return advert;
    }

    //Image Operations
    public async Task AddImagesAsync(List<AdvertImage> images, CancellationToken cancellationToken = default)
    {
        await context.AdvertImageDbSet.AddRangeAsync(images, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AdvertImage?> GetImageByIdAsync(int imageId, CancellationToken cancellationToken = default)
    {
        return await context.AdvertImageDbSet
            .FirstOrDefaultAsync(img => img.Id == imageId, cancellationToken);
    }

    public async Task<AdvertImage?> DeleteImageAsync(int imageId, CancellationToken cancellationToken = default)
    {
        var image = await GetImageByIdAsync(imageId, cancellationToken);
        if (image == null || image.IsPrimary) return null;
        context.AdvertImageDbSet.Remove(image);
        await context.SaveChangesAsync(cancellationToken);
        return image;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> AdvertExistById(int advertId, CancellationToken cancellationToken = default)
    {
        return await context.AdvertDbSet.AnyAsync(a => a.Id == advertId, cancellationToken);
    }

    // Repository'de
    public async Task<(List<Advert> adverts, int totalCount)> FilterAdvertsAsync(AdvertFilterDto filter, int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.AdvertDbSet
            .Include(a => a.Animal)
            .Include(a => a.Seller)
            .Include(a => a.Images.Where(img => img.IsPrimary))
            .AsQueryable();

        // Filtreler
        if (!string.IsNullOrWhiteSpace(filter.City))
            query = query.Where(a => a.Address.City == filter.City);

        if (!string.IsNullOrWhiteSpace(filter.District))
            query = query.Where(a => a.Address.District == filter.District);

        if (filter.MinPrice.HasValue)
            query = query.Where(a => a.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(a => a.Price <= filter.MaxPrice.Value);

        if (filter.AnimalKind.HasValue)
            query = query.Where(a => a.Animal.Kind == filter.AnimalKind.Value);

        if (filter.Gender.HasValue)
            query = query.Where(a => a.Animal.Gender == filter.Gender.Value);

        if (filter.MinAge.HasValue)
            query = query.Where(a => a.Animal.Age >= filter.MinAge.Value);

        if (filter.MaxAge.HasValue)
            query = query.Where(a => a.Animal.Age <= filter.MaxAge.Value);

        if (!string.IsNullOrWhiteSpace(filter.BusinessName))
            query = query.Where(a => a.Seller.BusinessName.Contains(filter.BusinessName));

        var totalCount = await query.CountAsync(cancellationToken);

        var adverts = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (adverts, totalCount);
    }
}