using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class StudentAnswerConfiguration : IEntityTypeConfiguration<QuestionAnswer>
{
    public void Configure(EntityTypeBuilder<QuestionAnswer> builder)
    {
        builder.ToTable("StudentAnswers");
    }
}
