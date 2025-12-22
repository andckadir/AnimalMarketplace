using AnimalMarketplace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimalMarketplace.Database.Configurations;

public class SellerConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        builder.ToTable("sellers");
        
        builder.HasKey(s=>s.Id);
        
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");
        
        builder.Property(s => s.BusinessName)
            .HasColumnName("business_name")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(s => s.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        
        builder.HasIndex(s => s.UserId).IsUnique();
        
        
    }
}