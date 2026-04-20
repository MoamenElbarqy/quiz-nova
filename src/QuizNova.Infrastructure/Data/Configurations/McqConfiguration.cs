using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class McqConfiguration : IEntityTypeConfiguration<Mcq>
{
    public void Configure(EntityTypeBuilder<Mcq> builder)
    {
        builder.ToTable("MCQs");

        builder.HasMany(q => q.Choices)
            .WithOne()
            .HasForeignKey(c => c.QuestionId)
            .HasPrincipalKey(q => q.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(q => q.CorrectChoice)
            .WithMany()
            .HasForeignKey(q => q.CorrectChoiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
