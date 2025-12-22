using AnimalMarketplace.Models;

namespace AnimalMarketplace.Database.DTO;

public class AdvertFilterDto
{
    public string? City { get; set; }
    public string? District { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public AnimalKind? AnimalKind { get; set; }
    public Gender? Gender { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? BusinessName { get; set; }
}