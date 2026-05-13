using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.Admins.Events;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users.Admins;

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
            UserRole.Admin,
            refreshTokens)
    {
    }

    public static Result<Admin> Create(
        Guid id,
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens)
    {
        var validationError = ValidateCommon(personalInformation, UserRole.Admin);

        if (validationError.IsError)
        {
            return validationError.TopError;
        }

        var admin = new Admin(id, personalInformation, refreshTokens);
        admin.AddDomainEvent(new AdminCreatedEvent(id));
        return admin;
    }

    public Result<Updated> Update(PersonalInformation personalInformation)
    {
        var updateResult = UpdateCommon(personalInformation, UserRole.Admin);
        if (!updateResult.IsError)
        {
            AddDomainEvent(new AdminUpdatedEvent(Id));
        }

        return updateResult;
    }

    public Result<Deleted> Delete()
    {
        AddDomainEvent(new AdminDeletedEvent(Id));
        return Result.Deleted;
    }
}
