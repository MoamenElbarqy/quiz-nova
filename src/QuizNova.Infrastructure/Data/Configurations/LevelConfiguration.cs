using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Levels;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class LevelConfiguration : IEntityTypeConfiguration<Level>
{
    public void Configure(EntityTypeBuilder<Level> builder)
    {
        builder.ToTable("Levels");
    }
}
