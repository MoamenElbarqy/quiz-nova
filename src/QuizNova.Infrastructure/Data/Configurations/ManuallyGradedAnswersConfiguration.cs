using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class ManuallyGradedAnswersConfiguration : IEntityTypeConfiguration<ManuallyGradedAnswers>
{
    public void Configure(EntityTypeBuilder<ManuallyGradedAnswers> builder)
    {
        builder.ToTable("ManuallyGradedAnswers");

        builder.Property(a => a.Score)
            .IsRequired(false);
    }
}
