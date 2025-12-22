using System.Text.Json.Serialization;

namespace AnimalMarketplace.Models;

public class AdvertImage
{
    public int Id { get; set; }
    public string Url { get; set; }  // Cloud storage URL
    public string PublicId { get; set; }  // Cloud provider ID (silmek için)
    public int Order { get; set; }  // Sıralama (1. fotoğraf, 2. fotoğraf, vb.)
    public bool IsPrimary { get; set; }  // Ana fotoğraf mı?
    public DateTime UploadedAt { get; set; }
    
    // Foreign Key
    public int AdvertId { get; set; }
    
    // Navigation Property
    public Advert Advert { get; set; }
}