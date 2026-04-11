using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users;

public class Admin : User
{
    private Admin()
    {
    }

    private Admin(
        Guid id,
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens)
        : base(
            id,
            personalInformation,
            Role.Admin,
            refreshTokens)
    {
    }

    public static Result<Admin> Create(
        Guid id,
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens)
    {
        var validationError = ValidateCommon(personalInformation, Role.Admin);

        if (validationError.IsError)
        {
            return validationError.TopError;
        }

        return new Admin(id, personalInformation, refreshTokens);
    }
}
