// Controllers/AdvertController.cs

using System.Security.Claims;
using AnimalMarketplace.Database.Dto;
using AnimalMarketplace.Database.DTO;
using AnimalMarketplace.Database.Dto.Response;
using AnimalMarketplace.Database.DTO.Response;
using AnimalMarketplace.Models;
using AnimalMarketplace.Repositories.Interfaces;
using AnimalMarketplace.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimalMarketplace.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdvertController(
    IImageService imageService,
    IAdvertRepository advertRepository,
    IValidator<AdvertCreateDto> validator,
    ILogger<AdvertController> logger,
    ISellerService sellerService)
    : ControllerBase
{
    [HttpPost("create")]
    [Authorize]
    [RequestSizeLimit(52428800)]
    public async Task<IActionResult> CreateAdvert(
        [FromForm] AdvertCreateDto advertCreateDto,
        [FromForm] List<IFormFile> images,
        CancellationToken cancellationToken)
    {
        // JWT token'dan authenticated user'ın ID'sini al
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int authenticatedUserId))
        {
            return Unauthorized(new { Message = "Invalid user authentication" });
        }

        // Bu user'ın seller olup olmadığını ve seller ID'sini kontrol et
        var isSeller = await sellerService.SellerExistByUserIdAsync(authenticatedUserId, cancellationToken);

        if (!isSeller)
        {
            return StatusCode(403, new { Message = "Only sellers can create adverts" });
        }
        
        //DTO Validation
        var validationResult = await validator.ValidateAsync(advertCreateDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Message = "Validation Error",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }
        
        //Check Number of Images
        if (images == null || images.Count == 0)
        {
            return BadRequest(new { Message = "You must upload at least 1 image" });
        }
        
        if (images.Count > 10)
        {
            return BadRequest(new { Message = "You can upload maximum of 10 images." });
        }
        
        try
        {
            var result = await imageService.ValidateImagesAsync(images); 

            //At least 1 valid image
            if (result.validImages.Count == 0)
            {
                return BadRequest(new
                {
                    Message = "No valid images were found. The listing could not be created.",
                    Errors = result.validationErrors
                });
            }

            //Upload to Cloudinary
            var uploadedImages = new List<AdvertImage>();
            var uploadErrors = new List<string>();

            for (int i = 0; i < result.validImages.Count; i++)
            {
                var image = result.validImages[i];

                try
                {
                    var uploadResult = await imageService.UploadImageAsync(image);

                    if (uploadResult.Error != null)
                    {
                        uploadErrors.Add($"'{image.FileName}' Failed to load: {uploadResult.Error.Message}");
                        continue;
                    }

                    uploadedImages.Add(new AdvertImage
                    {
                        Url = uploadResult.SecureUrl.ToString(),
                        PublicId = uploadResult.PublicId,
                        Order = i + 1,
                        IsPrimary = i == 0,
                        UploadedAt = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Cloudinary installation error: {FileName}", image.FileName);
                    uploadErrors.Add($"'{image.FileName}' Unexpected error during loading.");
                }
            }

            //At least one image must be uploaded
            if (!uploadedImages.Any())
            {
                return BadRequest(new
                {
                    Message = "Images could not be uploaded. The listing could not be created.",
                    Errors = uploadErrors
                });
            }

            var serverResult = await sellerService.GetSellerIdByUserIdAsync(authenticatedUserId,cancellationToken);
            if (!serverResult.IsSuccess)
            {
                return BadRequest(serverResult.ErrorMessage);
            }
            var advert = new Advert(serverResult.Data,advertCreateDto);
            var createdAdvert = await advertRepository.CreateAsync(advert, uploadedImages, cancellationToken);
            var advertWithDetails = await advertRepository.GetByIdWithDetailsAsync(createdAdvert.Id, cancellationToken);

            return CreatedAtAction(nameof(GetAdvertById), new { id = createdAdvert.Id }, new
            {
                Message = "Advert created successfully",
                Advert = new AdvertDetailsDto(advertWithDetails)
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Advert creation failed");
            return StatusCode(500, new { Message = "Server Error", Detail = ex.Message });
        }
    }
    
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetAdvertById(int id, CancellationToken cancellationToken)
    {
        var advert = await advertRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (advert == null) return NotFound(new { Message = "Advert  not found" });
        return Ok(new AdvertDetailsDto(advert));
    }

    [AllowAnonymous]
    [HttpPost("filter")]
    public async Task<ActionResult<PagedResult<AdvertSimpleDto>>> GetAllAdverts(
        [FromBody] AdvertFilterDto advertFilterDto,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var (adverts, totalCount) = await advertRepository
            .FilterAdvertsAsync(advertFilterDto, page, pageSize, cancellationToken);
    
        var response = adverts.Select(a => new AdvertSimpleDto(a)).ToList();
    
        return Ok(new PagedResult<AdvertSimpleDto>
        {
            Data = response,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    [Authorize]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateAdvert(int id, [FromBody] AdvertCreateDto advertUpdateDto,
        CancellationToken cancellationToken)
    {
        var advert = await advertRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (advert == null) return NotFound(new { Message = "Advert not found" });

        var validationResult = await validator.ValidateAsync(advertUpdateDto, cancellationToken);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        // Güncelleme mantığı (Address ve Animal ilklendirilmiş geliyor - Advert Constructor sayesinde)
        advert.Price = advertUpdateDto.Price;
        advert.Description = advertUpdateDto.Description;
        advert.Address.City = advertUpdateDto.City;
        advert.Address.District = advertUpdateDto.District;
        advert.Animal.Gender = advertUpdateDto.Gender;
        advert.Animal.Age = advertUpdateDto.Age;
        advert.Animal.Kind = advertUpdateDto.Kind;

        await advertRepository.UpdateAsync(advert, cancellationToken);

        return Ok(new { Message = "İlan güncellendi", Advert = new AdvertDetailsDto(advert) });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdvert(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();
            
            var userId = int.Parse(userIdClaim);
            
            var advert = await advertRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            if (advert == null) return NotFound(new { Message = "İlan bulunamadı" });

            if (userId != advert.Seller.UserId && !advert.Seller.User.IsAdmin)
            {
                return Unauthorized(new
                {
                    Message = "İlanı silmek için yetkiniz yok"
                });
            }
            
            // Önce Cloudinary temizliği
            if (advert.Images != null)
            {
                foreach (var image in advert.Images)
                {
                    await imageService.DeleteImageAsync(image.PublicId);
                }
            }

            // DB'den sil (Cascade sayesinde resimler ve animal da silinir)
            await advertRepository.DeleteAsync(id, cancellationToken);

            return Ok(new { Message = "İlan ve bağlı tüm veriler silindi" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "İlan silme hatası");
            return StatusCode(500, new { Message = "Sunucu hatası" });
        }
    }

    // ============ RESİM YÖNETİMİ (OPSIYONEL - SONRADAN EKLENEBİLİR) ============

    // IMAGE: Yeni resim ekleme
    [Authorize]
    [HttpPost("{advertId}/images")]
    public async Task<IActionResult> AddImagesToAdvert(int advertId, [FromForm] List<IFormFile> images,
        CancellationToken cancellationToken)
    {
        var advert = await advertRepository.GetByIdWithDetailsAsync(advertId, cancellationToken);
        if (advert == null) return NotFound(new { Message = "İlan bulunamadı" });

        if ((advert.Images?.Count ?? 0) + images.Count > 10)
            return BadRequest(new { Message = "Maksimum 10 resim sınırına ulaşıldı." });

        var hasAnyPrimary = advert.Images?.Any(i => i.IsPrimary) ?? false;
        var uploadedImages = new List<AdvertImage>();

        foreach (var image in images)
        {
            // (Burada IsValidImageAsync kontrolünü tekrar çağırabilirsin - Create'deki ile aynı)
            var uploadResult = await imageService.UploadImageAsync(image);
            if (uploadResult.Error == null)
            {
                uploadedImages.Add(new AdvertImage
                {
                    Url = uploadResult.SecureUrl.ToString(),
                    PublicId = uploadResult.PublicId,
                    AdvertId = advertId,
                    Order = (advert.Images?.Count ?? 0) + uploadedImages.Count + 1,
                    IsPrimary = !hasAnyPrimary && uploadedImages.Count == 0,
                    UploadedAt = DateTime.UtcNow
                });
            }
        }

        if (uploadedImages.Any()) await advertRepository.AddImagesAsync(uploadedImages, cancellationToken);

        return Ok(new { Message = "Resimler eklendi", Count = uploadedImages.Count });
    }

    // IMAGE: Resim silme (Repository'deki IsPrimary korumalı versiyon)
    [Authorize]
    [HttpDelete("images/{imageId}")]
    public async Task<IActionResult> DeleteImage(int imageId, CancellationToken cancellationToken)
    {
        var image = await advertRepository.GetImageByIdAsync(imageId, cancellationToken);
        if (image == null) return NotFound();

        // Repository'deki yeni kısıtlamalı metodumuz
        var result = await advertRepository.DeleteImageAsync(imageId, cancellationToken);

        if (result == null)
            return BadRequest(new { Message = "Ana resim (Primary) silinemez veya resim bulunamadı." });

        await imageService.DeleteImageAsync(result.PublicId);
        return Ok(new { Message = "Resim silindi" });
    }

    // IMAGE: Kapak fotoğrafı değiştirme
    [Authorize]
    [HttpPatch("images/{imageId}/set-primary")]
    public async Task<IActionResult> SetPrimaryImage(int imageId, CancellationToken cancellationToken)
    {
        var image = await advertRepository.GetImageByIdAsync(imageId, cancellationToken);
        if (image == null) return NotFound();

        var advert = await advertRepository.GetByIdWithDetailsAsync(image.AdvertId, cancellationToken);
        if (advert == null) return NotFound();

        foreach (var img in advert.Images)
        {
            img.IsPrimary = img.Id == imageId;
        }

        await advertRepository.SaveChangesAsync(cancellationToken);
        return Ok(new { Message = "Kapak fotoğrafı güncellendi" });
    }
}