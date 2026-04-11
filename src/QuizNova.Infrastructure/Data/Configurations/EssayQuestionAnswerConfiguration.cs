using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.QuizAttempts.Answers.Essay;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class EssayQuestionAnswerConfiguration : IEntityTypeConfiguration<EssayQuestionAnswer>
{
    public void Configure(EntityTypeBuilder<EssayQuestionAnswer> builder)
    {
        builder.ToTable("EssayQuestionAnswers");
    }
}
