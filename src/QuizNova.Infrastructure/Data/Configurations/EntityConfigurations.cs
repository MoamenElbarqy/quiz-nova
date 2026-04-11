using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Common;

namespace QuizNova.Infrastructure.Data.Configurations;

internal sealed class EntityConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.ToTable("Entity");
    }
}
