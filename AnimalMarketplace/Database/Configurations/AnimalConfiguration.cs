using AnimalMarketplace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimalMarketplace.Database.Configurations;

public class AnimalConfiguration : IEntityTypeConfiguration<Animal>
{
    public void Configure(EntityTypeBuilder<Animal> builder)
    {
        builder.ToTable("animals", t =>
        {
            t.HasCheckConstraint("ck_age_valid", "age between 0 and 100");
        });
        
        builder.HasKey(a=>a.Id);
        
        builder.Property(a => a.AdvertId)
            .HasColumnName("advert_id")
            .IsRequired();
        
        builder.Property(a=>a.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(a => a.Age)
            .HasColumnName("age")
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(a => a.Gender)
            .HasConversion<string>()
            .HasColumnName("gender")
            .HasColumnType("varchar(6)")
            .IsRequired();

        builder.Property(a => a.Kind)
            .HasConversion<string>()
            .HasColumnName("kind")
            .HasColumnType("varchar(10)")
            .IsRequired();
        
        builder.Property(a => a.AdvertId)
            .HasColumnName("advert_id")
            .IsRequired();
        
        builder.HasOne(a => a.Advert)
            .WithOne(ad => ad.Animal)
            .HasForeignKey<Animal>(a => a.AdvertId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}