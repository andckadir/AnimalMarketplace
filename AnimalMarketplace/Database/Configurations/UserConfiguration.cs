using AnimalMarketplace.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnimalMarketplace.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(u => u.Name)
            .IsRequired()
            .HasColumnType("varchar(50)")
            .HasColumnName("name");

        builder.Property(u => u.Surname)
            .IsRequired()
            .HasColumnType("varchar(50)")
            .HasColumnName("surname");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasColumnType("varchar(100)")
            .HasColumnName("email");
        
        builder.HasIndex(u => u.Email)
            .IsUnique(true);

        builder.Property(u => u.Phone)
            .HasColumnType("varchar(20)")
            .HasColumnName("phone")
            .IsRequired();

        builder.Property(u => u.RegisterDate)
            .HasDefaultValueSql("CURRENT_DATE")
            .HasColumnName("register_date")
            .IsRequired();

        builder.Property(u => u.Gender)
            .HasConversion<string>()
            .HasMaxLength(10)
            .HasColumnName("gender")
            .IsRequired();
        
        builder.Property(u => u.IsAdmin)
            .HasColumnName("is_admin")
            .HasDefaultValue(false);
        
        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasColumnType("varchar(255)")
            .IsRequired();
        
        builder.HasOne(u=>u.Seller)
            .WithOne(s=>s.User)
            .HasForeignKey<Seller>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}