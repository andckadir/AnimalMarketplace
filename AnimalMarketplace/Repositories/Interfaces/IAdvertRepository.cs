using AnimalMarketplace.Database.DTO;
using AnimalMarketplace.Models;

namespace AnimalMarketplace.Repositories.Interfaces;

public interface IAdvertRepository
{
    Task<Advert?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Advert>> GetAllWithPrimaryImageAsync(CancellationToken cancellationToken = default);
    Task<Advert> CreateAsync(Advert advert, List<AdvertImage> images, CancellationToken cancellationToken = default);
    Task<Advert> UpdateAsync(Advert advert, CancellationToken cancellationToken = default);
    Task<Advert?> DeleteAsync(int id, CancellationToken cancellationToken = default);

    //Image Operations
    Task AddImagesAsync(List<AdvertImage> images, CancellationToken cancellationToken = default);
    Task<AdvertImage?> GetImageByIdAsync(int imageId, CancellationToken cancellationToken = default);
    Task<AdvertImage?> DeleteImageAsync(int imageId, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> AdvertExistById(int advertId, CancellationToken cancellationToken = default);

    Task<(List<Advert> adverts, int totalCount)> FilterAdvertsAsync(AdvertFilterDto filter, int pageNumber,
        int pageSize, CancellationToken cancellationToken = default);
}