using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.QuizAttempts.Answers.TrueFalseAnswer;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class TfAnswerConfiguration : IEntityTypeConfiguration<TfAnswer>
{
    public void Configure(EntityTypeBuilder<TfAnswer> builder)
    {
        builder.ToTable("TfAnswers");
    }
}
