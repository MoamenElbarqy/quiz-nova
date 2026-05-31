using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Courses.Enums;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasDefaultValue(CourseStatus.Active)
            .IsRequired();

        builder.Navigation(c => c.Quizzes)
            .HasField("_quizzes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(c => c.Enrollments)
            .HasField("_enrollments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(c => c.Instructor)
            .WithMany(i => i.Courses)
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
