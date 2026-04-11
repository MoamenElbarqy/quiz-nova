using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Domain.Entities.Departments;

public sealed class Department : AuditableEntity
{
    private readonly List<Student> _students;

    private readonly List<Course> _courses;

    private Department()
    {
        _students = new List<Student>();
        _courses = new List<Course>();
    }

    private Department(Guid id, string name, List<Student> students, List<Course> courses)
       : base(id)
    {
        Name = name;
        _students = students;
        _courses = courses;
    }

    public string Name { get; private set; } = string.Empty;


    public IEnumerable<Student> Students => _students.AsReadOnly();

    public IEnumerable<Course> Courses => _courses.AsReadOnly();

    public static Result<Department> Create(
        Guid id,
        string name,
        List<Student> students,
        List<Course> courses)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return DepartmentErrors.NameRequired;
        }

        return new Department(id, name, students, courses);
    }
}
