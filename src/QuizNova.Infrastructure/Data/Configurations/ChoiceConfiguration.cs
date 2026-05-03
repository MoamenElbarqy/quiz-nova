using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Quizzes.Questions.Mcq.Choices;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class ChoiceConfiguration : IEntityTypeConfiguration<Choice>
{
    public void Configure(EntityTypeBuilder<Choice> builder)
    {
        builder.ToTable("Choices");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Text)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(c => c.Question)
            .WithMany()
            .HasForeignKey(c => c.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
