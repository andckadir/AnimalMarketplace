using AnimalMarketplace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimalMarketplace.Database.Configurations;

public class AdvertConfiguration : IEntityTypeConfiguration<Advert>
{
    public void Configure(EntityTypeBuilder<Advert> builder)
    {
        builder.ToTable("adverts", t =>
        {
            t.HasCheckConstraint("ck_advert_price_valid", "price >= 0 ");
        });
        
        builder.HasOne(ad=>ad.Animal)
            .WithOne(an=>an.Advert)
            .HasForeignKey<Animal>(a=>a.AdvertId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(ad => ad.Seller)
            .WithMany(s => s.Adverts)
            .HasForeignKey(ad => ad.SellerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(a => a.SellerId)
            .HasColumnName("seller_id")
            .IsRequired();
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");
        
        builder.Property(a=>a.Date)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_DATE")
            .HasColumnName("date");
        
        builder.Property(a=>a.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("price");
        
        builder.Property(a=>a.State)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(AdvertState.Active)
            .HasMaxLength(15)
            .HasColumnName("state");
        
        builder.Property(a=>a.Description)
            .IsRequired()
            .HasColumnName("description")
            .HasColumnType("varchar(500)");
        
        builder.Property(a=>a.Title)
            .IsRequired()
            .HasColumnName("title")
            .HasColumnType("varchar(50)");

        builder.OwnsOne(a => a.Address, address =>
        {
            address.Property(ad => ad.City)
                .IsRequired()
                .HasColumnType("varchar(50)")
                .HasColumnName("city");
            
            address.Property(ad=>ad.District)
                .IsRequired()
                .HasColumnType("varchar(50)")
                .HasColumnName("district");
        });
        
    }
}