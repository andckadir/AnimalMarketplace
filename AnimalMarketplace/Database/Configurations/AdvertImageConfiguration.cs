using AnimalMarketplace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimalMarketplace.Database.Configurations;

public class AdvertImageConfiguration : IEntityTypeConfiguration<AdvertImage>
{
    public void Configure(EntityTypeBuilder<AdvertImage> builder)
    {
        builder.ToTable("advert_images");
        
        builder.HasKey(ai => ai.Id);
        
        builder.Property(ai => ai.Id)
        .ValueGeneratedOnAdd()
        .HasColumnName("id");
        
        builder.Property(ai => ai.Url)
        .IsRequired()
        .HasColumnType("varchar(500)")
        .HasColumnName("url");
        
        builder.Property(ai => ai.PublicId)
        .HasColumnType("varchar(200)")
        .HasColumnName("public_id");
        
        builder.Property(ai => ai.Order)
        .IsRequired()
        .HasDefaultValue(1)
        .HasColumnName("order");
        
        builder.Property(ai => ai.IsPrimary)
        .IsRequired()
        .HasDefaultValue(false)
        .HasColumnName("is_primary");
        
        builder.Property(ai => ai.UploadedAt)
        .IsRequired()
        .HasDefaultValueSql("CURRENT_TIMESTAMP")
        .HasColumnName("uploaded_at");
        
        builder.Property(ai => ai.AdvertId)
        .IsRequired()
        .HasColumnName("advert_id");
        
        builder.HasOne(ai => ai.Advert)
        .WithMany(a => a.Images)
        .HasForeignKey(ai => ai.AdvertId)
        .OnDelete(DeleteBehavior.Cascade);
        
        // Index
        builder.HasIndex(ai => ai.AdvertId);
    
        
    }
}