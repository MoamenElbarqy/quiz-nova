using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users;

public abstract class User : Entity
{
    private readonly List<RefreshToken> _refreshTokens;

    protected User()
    {
        _refreshTokens = new List<RefreshToken>();
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

    public PersonalInformation PersonalInformation { get; private set; } = null!;

    public UserRole UserRole { get; private set; }

    public IEnumerable<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    protected static Result<Validated> ValidateCommon(
        PersonalInformation personalInformation,
        UserRole userRole)
    {
        var personalInformationError = PersonalInformation.Validate(personalInformation);

        if (personalInformationError.IsError)
        {
            return personalInformationError;
        }

        if (!Enum.IsDefined(userRole))
        {
            return PersonalInformationErrors.RoleInvalid;
        }

        return Result.Validated;
    }

    protected Result<Updated> UpdateCommon(
        PersonalInformation personalInformation,
        UserRole userRole)
    {
        var validationError = ValidateCommon(personalInformation, userRole);

        if (validationError.IsError)
        {
            return validationError.TopError;
        }

        PersonalInformation = personalInformation;

        return Result.Updated;
    }
}
