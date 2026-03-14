using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Departments;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Quizzes;

namespace QuizNova.Domain.Entities.Colleges;

public class College : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public List<Admin> Admins { get; private set; } = [];

    public List<Department> Departments { get; private set; } = [];

    public List<Course> Courses { get; private set; } = [];

    public List<Student> Students { get; private set; } = [];

    public List<Quiz> Quizzes { get; private set; } = [];

    private College()
    {
    }

    private College(Guid id, string name)
        : base(id)
    {
        Name = name;
    }

    public static Result<College> Create(Guid id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CollegeErrors.NameRequired;
        }

        return new College(id, name);
    }
}
