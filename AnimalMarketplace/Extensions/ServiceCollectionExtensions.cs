using AnimalMarketplace.Services;

namespace AnimalMarketplace.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        
        services.AddScoped<IImageService, CloudinaryImageService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISellerService, SellerService>();
    }
}