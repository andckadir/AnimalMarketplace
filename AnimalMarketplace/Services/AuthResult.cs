using AnimalMarketplace.Models;

namespace AnimalMarketplace.Services;

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public string Token { get; set; }
    public User User { get; set; }
    
    public static AuthResult Success(string token, User user)
        => new() { IsSuccess = true, Token = token, User = user };
    
    public static AuthResult Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}