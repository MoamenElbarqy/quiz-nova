using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class TrueFalseQuestionConfiguration : IEntityTypeConfiguration<TrueFalseQuestion>
{
    public void Configure(EntityTypeBuilder<TrueFalseQuestion> builder)
    {
        builder.ToTable("TrueFalseQuestions");
    }
}
