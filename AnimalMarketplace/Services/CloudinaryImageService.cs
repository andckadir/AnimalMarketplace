using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace AnimalMarketplace.Services;

public class CloudinaryImageService : IImageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageService(IConfiguration config)
    {
        var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
    {
        if (file.Length <= 0) return null;

        await using var stream = file.OpenReadStream();
        
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "animal-marketplace/adverts",
            Transformation = new Transformation()
                .Width(1200).Height(800).Crop("limit")
                .Quality("auto")
        };

        return await _cloudinary.UploadAsync(uploadParams);
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result.Result == "ok";
    }   
    
    public async Task<bool> IsValidImageAsync(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            var header = new byte[12]; // WEBP için 12 byte gerekli
            var bytesRead = await stream.ReadAsync(header, 0, 12);

            if (bytesRead < 8) return false; // Minimum 8 byte

            // JPEG (FF D8 FF)
            if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
                return true;

            // PNG (89 50 4E 47)
            if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47)
                return true;

            // WEBP (RIFF....WEBP - 12 byte gerekli)
            if (bytesRead >= 12 &&
                header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50)
                return true;

            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<(List<IFormFile> validImages,List<string> validationErrors)> ValidateImagesAsync(List<IFormFile> images)
    {
        //Image Validation
        var validImages = new List<IFormFile>();
        var validationErrors = new List<string>();

        foreach (var image in images)
        {
            //Size Control
            if (image.Length > 5 * 1024 * 1024)
            {
                validationErrors.Add($"'{image.FileName}' exceeds 5MB limit");
                continue;
            }

            //Format Control
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(image.ContentType.ToLower()))
            {
                validationErrors.Add($"'{image.FileName}' invalid format. Allowed: JPEG, PNG, WEBP");
                continue;
            }

            // Magic byte control
            if (!await IsValidImageAsync(image))
            {
                validationErrors.Add($"'{image.FileName}' invalid image doc");
                continue;
            }

            validImages.Add(image);
        }

        return  (validImages, validationErrors);
    }
}