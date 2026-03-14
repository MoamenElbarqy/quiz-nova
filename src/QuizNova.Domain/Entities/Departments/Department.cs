using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Colleges;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Departments;

namespace QuizNova.Domain.Entities.Departments;

public class Department : AuditableEntity
{
    public Guid CollegeId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public College? College { get; private set; }

    public List<Student> Students { get; private set; } = [];

    public List<Course> Courses { get; private set; } = [];

    private Department()
    {
    }

    private Department(Guid id, Guid collegeId, string name)
        : base(id)
    {
        CollegeId = collegeId;
        Name = name;
    }

    public static Result<Department> Create(Guid id, Guid collegeId, string name)
    {
        if (collegeId == Guid.Empty)
        {
            return DepartmentErrors.CollegeIdRequired;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return DepartmentErrors.NameRequired;
        }

        return new Department(id, collegeId, name);
    }
}
