using CloudinaryDotNet.Actions;

namespace AnimalMarketplace.Services;

public interface IImageService
{ 
    Task<ImageUploadResult> UploadImageAsync(IFormFile file); 
    Task<bool> DeleteImageAsync(string publicId);
    Task<bool> IsValidImageAsync(IFormFile file);
    Task<(List<IFormFile> validImages, List<string> validationErrors)> ValidateImagesAsync(List<IFormFile> images);
}
