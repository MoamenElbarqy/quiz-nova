using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Departments;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.Colleges;

public sealed class College : AuditableEntity
{
    private readonly List<Admin> _admins;

    private readonly List<Department> _departments;

    private readonly List<Course> _courses;

    private readonly List<Student> _students;

    private readonly List<Quiz> _quizzes;

    private College(
        Guid id,
        string name,
        List<Admin> admins,
        List<Department> departments,
        List<Course> courses,
        List<Student> students,
        List<Quiz> quizzes)
        : base(id)
    {
        Name = name;
        _admins = admins;
        _departments = departments;
        _courses = courses;
        _students = students;
        _quizzes = quizzes;
    }

    public string Name { get; private set; } = string.Empty;

    public IEnumerable<Admin> Admins => _admins.AsReadOnly();

    public IEnumerable<Department> Departments => _departments.AsReadOnly();

    public IEnumerable<Course> Courses => _courses.AsReadOnly();

    public IEnumerable<Student> Students => _students.AsReadOnly();

    public IEnumerable<Quiz> Quizzes => _quizzes.AsReadOnly();

    public static Result<College> Create(
        Guid id,
        string name,
        List<Admin> admins,
        List<Department> departments,
        List<Course> courses,
        List<Student> students,
        List<Quiz> quizzes)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CollegeErrors.NameRequired;
        }

        return new College(id, name, admins, departments, courses, students, quizzes);
    }
}
