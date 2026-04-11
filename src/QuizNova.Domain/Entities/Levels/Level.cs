using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Domain.Entities.Levels;

public sealed class Level : AuditableEntity
{
    private readonly List<Student> _students;

    private Level()
    {
        _students = new List<Student>();
    }

    private Level(Guid id, string name, List<Student> students)
        : base(id)
    {
        Name = name;
        _students = students;
    }

    public string Name { get; private set; } = string.Empty;

    public IEnumerable<Student> Students => _students.AsReadOnly();

    public static Result<Level> Create(Guid id, string name, List<Student> students)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return LevelErrors.NameRequired;
        }

        return new Level(id, name, students);
    }
}
