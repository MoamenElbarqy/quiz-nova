using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.QuizAttempts;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.ToTable("QuizAttempts");
    }
}
