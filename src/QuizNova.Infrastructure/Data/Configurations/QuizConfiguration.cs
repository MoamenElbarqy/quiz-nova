using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Quizzes;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("Quizzes");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Navigation(q => q.Questions)
            .HasField("_questions")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
            
        builder.HasOne(q => q.Course)
            .WithMany(c => c.Quizzes)
            .HasForeignKey(q => q.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
