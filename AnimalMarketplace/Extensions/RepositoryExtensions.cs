using AnimalMarketplace.Repositories.Implementations;
using AnimalMarketplace.Repositories.Interfaces;

namespace AnimalMarketplace.Extensions;

public static class RepositoryExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISellerRepository, SellerRepository>();
        services.AddScoped<IAdvertRepository, AdvertRepository>();
    }
}