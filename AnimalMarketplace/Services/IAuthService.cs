using AnimalMarketplace.Database.DTO;

namespace AnimalMarketplace.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(UserCreateDto userCreateDto, CancellationToken cancellationToken = default);
    Task<AuthResult> LoginAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken = default);
    Task<AuthResult> ChangePasswordAsync(int userId, string oldPassword, string newPassword,
        CancellationToken cancellationToken);
    Task<AuthResult> VerifyPasswordAsync(int userId, string password, CancellationToken cancellationToken = default);
}