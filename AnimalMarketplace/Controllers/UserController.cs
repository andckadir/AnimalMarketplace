using System.Security.Claims;
using AnimalMarketplace.Database.Dto;
using AnimalMarketplace.Database.DTO;
using AnimalMarketplace.Database.Dto.Response;
using AnimalMarketplace.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnimalMarketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(
        IAuthService authService,
        IUserService userService,
        IValidator<LoginRequestDto> loginValidator,
        IValidator<UserCreateDto> registerValidator,
        IValidator<ChangePasswordDto> changePasswordValidator,
        IValidator<UserUpdateDto> updateValidator) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Create(UserCreateDto userCreateDto, CancellationToken cancellationToken)
        {
            var validationResult = await registerValidator.ValidateAsync(userCreateDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var authResult = await authService.RegisterAsync(userCreateDto, cancellationToken);
            if (!authResult.IsSuccess)
            {
                return BadRequest(authResult.ErrorMessage);
            }

            var user = authResult.User;

            return Ok(new
            {
                Token = authResult.Token,
                Message = "User successfully registered",
                User = new { user.Id, user.Name, user.Surname, user.Email, user.IsAdmin },
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto,
            CancellationToken cancellationToken)
        {
            var validationResult = await loginValidator.ValidateAsync(loginRequestDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var authResult = await authService.LoginAsync(loginRequestDto, cancellationToken);
            if (!authResult.IsSuccess)
            {
                return Unauthorized(authResult.ErrorMessage);
            }

            var user = authResult.User;
            return Ok(new
            {
                Token = authResult.Token,
                Message = "User successfully logged in",
                User = new { user.Id, user.Name, user.Surname, user.Email, user.IsAdmin }
            });
        }

        [Authorize]
        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto,
            CancellationToken cancellationToken)
        {
            var validationResult = await changePasswordValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(error => error.ErrorMessage));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var result =
                await authService.ChangePasswordAsync(userId, dto.OldPassword, dto.NewPassword, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(new { Message = result.ErrorMessage });
            }

            return Ok(new
            {
                Message = "Password successfully updated.",
                Token = result.Token
            });
        }

        [Authorize]
        [HttpPatch("update")]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto userUpdateDto,
            CancellationToken cancellationToken)
        {
            var validationResult = await updateValidator.ValidateAsync(userUpdateDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(error => error.ErrorMessage));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var updateResult = await userService.UpdateUserAsync(userId, userUpdateDto, cancellationToken);

            if (!updateResult.IsSuccess)
            {
                return BadRequest(new { Message = updateResult.ErrorMessage });
            }

            return Ok(new
                {
                    Message = "User successfully updated",
                    token = updateResult.Token,
                }
            );
        }
        
        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody]DeleteRequestDto dto,CancellationToken cancellationToken)
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
            
            var result = await userService.DeleteUserAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok("User successfully deleted") : BadRequest(result.ErrorMessage);
        }
        
        [Authorize]
        [HttpGet("get")]
        public async Task<IActionResult> GetUserDetail(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            
            var result = await userService.GetUserByIdAsync(userId, cancellationToken);
            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }
        
            return Ok(new UserResponseDto(result.Data));
        }

        [Authorize]
        [HttpPost("favorite")]
        public async Task<IActionResult> AddFavorite([FromQuery] int advertId, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            var result = await userService.AddFavoriteAsync(userId, advertId, cancellationToken);
            return result.IsSuccess ? Ok("Advert successfully added into favorites") : BadRequest(result.ErrorMessage);
        }
        
        [Authorize]
        [HttpDelete("favorite")]
        public async Task<IActionResult> RemoveFavorite([FromQuery] int advertId, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            var result = await userService.RemoveFavoriteAsync(userId, advertId, cancellationToken);
            return result.IsSuccess ? Ok("Advert successfully removed from favorites") : BadRequest(result.ErrorMessage);
        }
        
        [Authorize]
        [HttpGet("allfavorites")]
        public async Task<IActionResult> GetAllFavorites(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            var result = await userService.GetAllFavoritesAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result.Data.Select(f=>f.AdvertId).ToList()) : BadRequest(result.ErrorMessage);
        }
    }
}