using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users;

public class SuperAdmin : User
{
    private SuperAdmin(
        Guid id,
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens)
        : base(
            id,
            personalInformation,
            Role.SuperAdmin,
            refreshTokens)
    {
    }

    public static Result<SuperAdmin> Create(
        Guid id,
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens)
    {
        var validationError = ValidateCommon(personalInformation, Role.SuperAdmin);

        if (validationError.IsError)
        {
            return validationError.TopError;
        }

        return new SuperAdmin(id, personalInformation, refreshTokens);
    }
}
