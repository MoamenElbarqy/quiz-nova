using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.QuizAttempts.Answers.Mcq;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class McqAnswerConfiguration : IEntityTypeConfiguration<McqAnswer>
{
    public void Configure(EntityTypeBuilder<McqAnswer> builder)
    {
        builder.ToTable("McqAnswers");
    }
}
