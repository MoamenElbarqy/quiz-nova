using QuizNova.Domain.Common;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users;

public abstract class User : Entity
{
    private readonly List<RefreshToken> _refreshTokens;

    protected User()
    {
    }

    protected User(
        Guid id,
        PersonalInformation personalInformation,
        UserRole userRole,
        List<RefreshToken> refreshTokens)
        : base(id)
    {
        PersonalInformation = personalInformation;
        UserRole = userRole;
        _refreshTokens = refreshTokens;
    }

    public PersonalInformation PersonalInformation { get; protected set; } = null!;

    public UserRole UserRole { get; private set; }

    public IEnumerable<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
}
