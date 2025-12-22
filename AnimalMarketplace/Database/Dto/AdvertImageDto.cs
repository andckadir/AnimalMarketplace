using AnimalMarketplace.Models;

namespace AnimalMarketplace.Database.DTO.Response;

public class AdvertImageDto(AdvertImage image)
{
    public int Id { get; set; } = image.Id;
    public string Url { get; set; } = image.Url;
    public string PublicId { get; set; } = image.PublicId;
    public int Order { get; set; } = image.Order;
    public bool IsPrimary { get; set; } = image.IsPrimary;
    public DateTime UploadedAt { get; set; } = image.UploadedAt;
}