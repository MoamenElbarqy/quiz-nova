using QuizNova.Domain.Common;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users;

public abstract class User : Entity
{
    protected User()
    {
    }

    protected User(
        Guid id,
        PersonalInformation personalInformation,
        UserRole userRole)
        : base(id)
    {
        PersonalInformation = personalInformation;
        UserRole = userRole;
    }

    public PersonalInformation PersonalInformation { get; protected set; } = null!;

    public UserRole UserRole { get; private set; }
}
