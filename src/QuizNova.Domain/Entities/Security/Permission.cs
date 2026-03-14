using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.Security;

public class Permission : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    private Permission()
    {
    }

    private Permission(Guid id, string name, string description)
        : base(id)
    {
        Name = name;
        Description = description;
    }

    public static Result<Permission> Create(Guid id, string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return PermissionErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return PermissionErrors.DescriptionRequired;
        }

        return new Permission(id, name, description);
    }
}
