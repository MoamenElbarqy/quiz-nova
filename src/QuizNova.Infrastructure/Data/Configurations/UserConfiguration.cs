using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Users;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.OwnsOne(
            e => e.PersonalInformation,
            personalInformation =>
            {
                personalInformation.Property(p => p.Name).HasColumnName("Name").IsRequired();
                personalInformation.Property(p => p.Email).HasColumnName("Email").IsRequired();
                personalInformation.Property(p => p.Password).HasColumnName("Password").IsRequired();
                personalInformation.Property(p => p.PhoneNumber).HasColumnName("PhoneNumber").IsRequired();
            });

        builder.Navigation(e => e.PersonalInformation).IsRequired();
    }
}
