using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class ManuallyGradedQuestionConfiguration : IEntityTypeConfiguration<ManuallyGradedQuestion<string>>
{
    public void Configure(EntityTypeBuilder<ManuallyGradedQuestion<string>> builder)
    {
        builder.ToTable("ManuallyGradedQuestions");

        builder.Property(q => q.Score)
            .IsRequired(false);
    }
}
