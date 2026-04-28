using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Users;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(user => user.UserRole)
            .HasColumnName("Role")
            .IsRequired();

        builder.OwnsOne(
            e => e.PersonalInformation,
            personalInformation =>
            {
                personalInformation.Property(p => p.Name).HasColumnName("Name").HasMaxLength(100).IsRequired();
                personalInformation.Property(p => p.Email).HasColumnName("Email").HasMaxLength(256).IsRequired();
                personalInformation.Property(p => p.Password).HasColumnName("Password").HasMaxLength(500).IsRequired();
                personalInformation.Property(p => p.PhoneNumber).HasColumnName("PhoneNumber").HasMaxLength(20).IsRequired();
            });

        builder.Navigation(e => e.PersonalInformation).IsRequired();

        builder.Navigation(u => u.RefreshTokens)
            .HasField("_refreshTokens")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
