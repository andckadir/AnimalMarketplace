using AnimalMarketplace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimalMarketplace.Database.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("favorites");

        builder.HasKey(f => new { f.UserId, f.AdvertId });

        builder.HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.UserId)
            .HasColumnName("user_id");

        builder.HasOne(f => f.Advert)
            .WithMany(a => a.Favorites)
            .HasForeignKey(f => f.AdvertId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(f => f.AdvertId)
            .HasColumnName("advert_id");
    }
}