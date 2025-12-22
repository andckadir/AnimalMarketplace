using AnimalMarketplace.Database.DTO;
using AnimalMarketplace.Models;
using AnimalMarketplace.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AnimalMarketplace.Services;

public class AuthService(IUserRepository userRepository, ITokenService tokenService) : IAuthService
{
    public async Task<AuthResult> RegisterAsync(UserCreateDto userCreateDto, CancellationToken cancellationToken)
    {
        if (await userRepository.UserExistsByEmailAsync(userCreateDto.Email, cancellationToken))
            return AuthResult.Failure("Email already exists");

        var user = new User(userCreateDto)
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreateDto.Password)
        };

        await userRepository.AddUserAsync(user, cancellationToken);

        var token = tokenService.CreateToken(user);
        return AuthResult.Success(token, user);
    }

    public async Task<AuthResult> LoginAsync(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByEmailAsync(loginRequestDto.Email, cancellationToken);
        if (user == null)
            return AuthResult.Failure("User  not found");

        if (!BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.PasswordHash))
            return AuthResult.Failure("Invalid password");

        var token = tokenService.CreateToken(user);
        return AuthResult.Success(token, user);
    }

    public async Task<AuthResult> ChangePasswordAsync(int userId, string oldPassword, string newPassword,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);
        if (user == null)
            return AuthResult.Failure("User not found");

        if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            return AuthResult.Failure("Current password is incorrect");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await userRepository.UpdateUserAsync(user, cancellationToken);
        var token = tokenService.CreateToken(user);
        return AuthResult.Success(token, user);
    }

    public async Task<AuthResult> VerifyPasswordAsync(int userId, string password, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);
        if (user == null)
            return AuthResult.Failure("User not found");
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return AuthResult.Failure("Password is incorrect");
        var token = tokenService.CreateToken(user);
        return AuthResult.Success(token,user);
    }

    
    // public async Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken ct)
    // {
    //     // Token yenileme mantığı
    // }

    // public async Task<AuthResult> ForgotPasswordAsync(string email, CancellationToken ct)
    // {
    //     // Email gönderme, token oluşturma
    // }
}