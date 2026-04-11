using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Quizzes.Questions.Essay;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class EssayQuestionConfiguration : IEntityTypeConfiguration<EssayQuestion>
{
    public void Configure(EntityTypeBuilder<EssayQuestion> builder)
    {
        builder.ToTable("EssayQuestions");
    }
}
