using AnimalMarketplace.Database.Configurations;
using AnimalMarketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimalMarketplace.Database.DbContexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> UserDbSet { get; set; }
    public DbSet<Advert> AdvertDbSet { get; set; }
    public DbSet<Favorite> FavoriteDbSet { get; set; }
    public DbSet<Animal> AnimalDbSet { get; set; }
    public DbSet<Seller> SellerDbSet { get; set; }
    public DbSet<AdvertImage> AdvertImageDbSet { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new AdvertConfiguration());
        modelBuilder.ApplyConfiguration(new FavoriteConfiguration());
        modelBuilder.ApplyConfiguration(new AnimalConfiguration());
        modelBuilder.ApplyConfiguration(new SellerConfiguration());
        modelBuilder.ApplyConfiguration(new AdvertImageConfiguration());
    }
}
