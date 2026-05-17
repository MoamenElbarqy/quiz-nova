using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class AutoGradedAnswerConfiguration : IEntityTypeConfiguration<AutoGradedAnswer>
{
    public void Configure(EntityTypeBuilder<AutoGradedAnswer> builder)
    {
        builder.ToTable("AutoGradedAnswers");

        builder.Property(a => a.IsCorrect)
            .IsRequired();
    }
}
