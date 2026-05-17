using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class ManuallyGradedQuestionConfiguration : IEntityTypeConfiguration<ManuallyGradedQuestion>
{
    public void Configure(EntityTypeBuilder<ManuallyGradedQuestion> builder)
    {
        builder.ToTable("ManuallyGradedQuestions");

        builder.Property(q => q.Score)
            .IsRequired(false);
    }
}
