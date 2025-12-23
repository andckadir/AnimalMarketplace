using AnimalMarketplace.Database.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AnimalMarketplace.Extensions;

public static class DatabaseServiceExtensions
{
    public static void AddDatabaseServices(this IServiceCollection services,IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Neon");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}