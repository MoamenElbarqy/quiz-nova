using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Common;

namespace QuizNova.Infrastructure.Data.Configurations;

internal sealed class AuditableEntityConfiguration : IEntityTypeConfiguration<AuditableEntity>
{
    public void Configure(EntityTypeBuilder<AuditableEntity> builder)
    {
        builder.ToTable("AuditableEntity");

        builder.Property(a => a.CreatedBy)
            .HasMaxLength(100);

        builder.Property(a => a.LastModifiedBy)
            .HasMaxLength(100);
    }
}
