using System.Security.Claims;
using AnimalMarketplace.Database.Dto;
using AnimalMarketplace.Database.Dto.Response;
using AnimalMarketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimalMarketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController(ISellerService sellerService,IUserService userService,IAuthService authService) : ControllerBase
    {
        [Authorize]
        [HttpPatch("update")]
        public async Task<IActionResult> Update([FromBody] CreateSellerDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.BusinessName))
            {
                return BadRequest("Business name is required");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var updateResult = await sellerService.UpdateSellerAsync(userId,dto,cancellationToken);

            return updateResult.IsSuccess ? Ok("Seller updated successfully") : BadRequest(updateResult.ErrorMessage);
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequestDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest("Password is required");
            }
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var authResult = await authService.VerifyPasswordAsync(userId, dto.Password, cancellationToken);
            if (!authResult.IsSuccess)
                return BadRequest(authResult.ErrorMessage);
            
            var result = await sellerService.DeleteSellerAsync(userId, cancellationToken);
                return result.IsSuccess ? Ok("Seller deleted successfully") : BadRequest(result.ErrorMessage);

        }

        [Authorize]
        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            
            var sellerResult = await sellerService.GetSellerByUserIdAsync(userId,cancellationToken);
            var userResult = await userService.GetUserByIdAsync(userId,cancellationToken);
            return sellerResult.IsSuccess ? Ok(new SellerResponseDto(sellerResult.Data,userResult.Data)) : BadRequest(sellerResult.ErrorMessage);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateSeller(CreateSellerDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.BusinessName))
            {
                return BadRequest("Business name is required");
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);
            
            var result = await sellerService.CreateSellerAsync(userId,dto,cancellationToken);
            return result.IsSuccess ? Ok("Seller created successfully") : BadRequest(result.ErrorMessage);
        }
    }
}
