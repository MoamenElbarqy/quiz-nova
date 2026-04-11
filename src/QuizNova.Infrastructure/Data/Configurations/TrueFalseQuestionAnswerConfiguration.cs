using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.QuizAttempts.Answers.TrueFalse;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class TrueFalseQuestionAnswerConfiguration : IEntityTypeConfiguration<TrueFalseQuestionAnswer>
{
    public void Configure(EntityTypeBuilder<TrueFalseQuestionAnswer> builder)
    {
        builder.ToTable("TrueFalseQuestionAnswers");
    }
}
