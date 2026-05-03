using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Users.Instructors;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("Instructors");

        builder.Navigation(i => i.Courses)
            .HasField("_courses")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(i => i.Quizzes)
            .HasField("_quizzes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
