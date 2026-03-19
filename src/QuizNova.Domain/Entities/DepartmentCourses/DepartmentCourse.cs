using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.DepartmentCourses;

public sealed class DepartmentCourse : AuditableEntity
{
    public Guid CourseId { get; private set; }

    public Guid DepartmentId { get; private set; }

    private DepartmentCourse()
    {
    }

    private DepartmentCourse(
        Guid id,
        Guid courseId,
        Guid departmentId)
        : base(id)
    {
        CourseId = courseId;
        DepartmentId = departmentId;
    }

    public static Result<DepartmentCourse> Create(Guid id, Guid courseId, Guid departmentId)
    {
        if (courseId == Guid.Empty)
        {
            return DepartmentCourseErrors.CourseIdRequired;
        }

        if (departmentId == Guid.Empty)
        {
            return DepartmentCourseErrors.DepartmentIdRequired;
        }

        return new DepartmentCourse(id, courseId, departmentId);
    }
}
