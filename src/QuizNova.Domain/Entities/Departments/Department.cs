using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Colleges;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.Departments;

public sealed class Department : AuditableEntity
{
    private readonly List<Student> _students;

    private readonly List<Course> _courses;

    private Department(Guid id, Guid collegeId, string name, List<Student> students, List<Course> courses)
       : base(id)
    {
        CollegeId = collegeId;
        Name = name;
        _students = students;
        _courses = courses;
    }

    public Guid CollegeId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public College? College { get; private set; }

    public IEnumerable<Student> Students => _students.AsReadOnly();

    public IEnumerable<Course> Courses => _courses.AsReadOnly();

    public static Result<Department> Create(
        Guid id,
        Guid collegeId,
        string name,
        List<Student> students,
        List<Course> courses)
    {
        if (collegeId == Guid.Empty)
        {
            return DepartmentErrors.CollegeIdRequired;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return DepartmentErrors.NameRequired;
        }

        return new Department(id, collegeId, name, students, courses);
    }
}
