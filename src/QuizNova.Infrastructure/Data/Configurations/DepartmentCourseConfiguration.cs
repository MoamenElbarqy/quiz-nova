using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.DepartmentCourses;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class DepartmentCourseConfiguration : IEntityTypeConfiguration<DepartmentCourse>
{
    public void Configure(EntityTypeBuilder<DepartmentCourse> builder)
    {
        builder.ToTable("DepartmentCourses");
    }
}
