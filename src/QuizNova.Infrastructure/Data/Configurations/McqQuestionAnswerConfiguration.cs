using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.QuizAttempts.Answers.Mcq;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class McqQuestionAnswerConfiguration : IEntityTypeConfiguration<McqQuestionAnswer>
{
    public void Configure(EntityTypeBuilder<McqQuestionAnswer> builder)
    {
        builder.ToTable("McqQuestionAnswers");
    }
}
