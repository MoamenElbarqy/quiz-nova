using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.StudentCourses;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class StudentCourseConfiguration : IEntityTypeConfiguration<StudentCourse>
{
    public void Configure(EntityTypeBuilder<StudentCourse> builder)
    {
        builder.ToTable("StudentCourses");
        builder.HasKey(sc => sc.Id);

        // Unique index to prevent duplicate enrollments
        builder.HasIndex(sc => new { sc.StudentId, sc.CourseId })
            .IsUnique();

        builder.HasOne(sc => sc.Student)
            .WithMany()
            .HasForeignKey(sc => sc.StudentId);

        builder.HasOne(sc => sc.Course)
            .WithMany()
            .HasForeignKey(sc => sc.CourseId);
    }
}
