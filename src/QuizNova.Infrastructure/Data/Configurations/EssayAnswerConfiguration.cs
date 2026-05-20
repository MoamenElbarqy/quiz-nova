using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers.EssayAnswer;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class EssayAnswerConfiguration : IEntityTypeConfiguration<EssayAnswer>
{
    public void Configure(EntityTypeBuilder<EssayAnswer> builder)
    {
        builder.ToTable("EssayAnswers");

        builder.Property(a => a.StudentResponse)
            .HasMaxLength(4000)
            .IsRequired();
    }
}
