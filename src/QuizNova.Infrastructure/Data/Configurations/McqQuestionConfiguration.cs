using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class McqQuestionConfiguration : IEntityTypeConfiguration<McqQuestion>
{
    public void Configure(EntityTypeBuilder<McqQuestion> builder)
    {
        builder.ToTable("McqQuestions");

        builder.HasMany(q => q.Choices)
            .WithOne()
            .HasForeignKey(c => c.QuestionId)
            .HasPrincipalKey(q => q.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(q => q.CorrectChoice)
            .WithMany()
            .HasForeignKey(q => q.CorrectChoiceId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_McqQuestions_Choices_CorrectChoiceId");
    }
}
