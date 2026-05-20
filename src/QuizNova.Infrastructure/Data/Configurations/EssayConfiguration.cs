using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class EssayConfiguration : IEntityTypeConfiguration<Essay>
{
    public void Configure(EntityTypeBuilder<Essay> builder)
    {
        builder.ToTable("Essays");

        builder.Property(q => q.AnswerReference)
            .HasMaxLength(2000)
            .IsRequired(false);
    }
}
