using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.Navigation(s => s.QuizAttempts)
            .HasField("_quizAttempts")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
