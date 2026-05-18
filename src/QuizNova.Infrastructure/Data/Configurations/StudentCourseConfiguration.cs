using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Enrollments;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");
        builder.HasKey(sc => sc.Id);

        // Unique index to prevent duplicate enrollments
        builder.HasIndex(sc => new { sc.StudentId, sc.CourseId })
            .IsUnique();

        builder.HasOne(sc => sc.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(sc => sc.StudentId);

        builder.HasOne(sc => sc.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(sc => sc.CourseId);
    }
}
