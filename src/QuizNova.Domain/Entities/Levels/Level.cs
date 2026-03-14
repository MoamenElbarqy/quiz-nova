using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities..Levels;

public class Level : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public List<Student> Students { get; private set; } = [];

    private Level()
    {
    }

    private Level(Guid id, string name)
        : base(id)
    {
        Name = name;
    }

    public static Result<Level> Create(Guid id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return LevelErrors.NameRequired;
        }

        return new Level(id, name);
    }
}
