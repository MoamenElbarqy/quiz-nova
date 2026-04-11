using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users;

public abstract class User : AuditableEntity
{
    private readonly List<RefreshToken> _refreshTokens;

    protected User()
    {
        _refreshTokens = new List<RefreshToken>();
    }

    protected User(
        Guid id,
        PersonalInformation personalInformation,
        Role role,
        List<RefreshToken> refreshTokens)
        : base(id)
    {
        PersonalInformation = personalInformation;
        Role = role;
        _refreshTokens = refreshTokens;
    }

    public PersonalInformation PersonalInformation { get; private set; } = null!;

    public Role Role { get; private set; }

    public IEnumerable<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    protected static Result<Validated> ValidateCommon(
        PersonalInformation personalInformation,
        Role role)
    {
        if (personalInformation is null)
        {
            return PersonalInformationErrors.Required;
        }

        var personalInformationError = PersonalInformation.Validate(personalInformation);

        if (personalInformationError.IsError)
        {
            return personalInformationError;
        }

        if (!Enum.IsDefined(role))
        {
            return PersonalInformationErrors.RoleInvalid;
        }

        return Result.Validated;
    }
}
