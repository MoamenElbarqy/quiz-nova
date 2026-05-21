using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Users.Admins;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.OwnsOne(
            a => a.PersonalInformation,
            personalInformation =>
            {
                personalInformation.Property(p => p.Name).HasColumnName("Name").HasMaxLength(100).IsRequired();
                personalInformation.Property(p => p.Email).HasColumnName("Email").HasMaxLength(256).IsRequired();
                personalInformation.Property(p => p.PhoneNumber).HasColumnName("PhoneNumber").HasMaxLength(20).IsRequired();
            });

        builder.Navigation(a => a.PersonalInformation).IsRequired();
    }
}
