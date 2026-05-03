using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.QuizAttempts;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.ToTable("QuizAttempts");
        builder.HasKey(qa => qa.Id);

        builder.Navigation(qa => qa.StudentAnswers)
            .HasField("_studentAnswers")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
            
        builder.HasOne(qa => qa.Student)
            .WithMany(s => s.QuizAttempts)
            .HasForeignKey(qa => qa.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(qa => qa.Quiz)
            .WithMany()
            .HasForeignKey(qa => qa.QuizId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
