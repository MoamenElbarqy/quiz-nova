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
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens)
    {
        var id = Guid.NewGuid();
        var admin = new Admin(id, personalInformation, refreshTokens);
        admin.AddDomainEvent(new AdminCreatedEvent(id));
        return admin;
    }

    public Result<Updated> Update(PersonalInformation personalInformation)
    {
        PersonalInformation = personalInformation;
        AddDomainEvent(new AdminUpdatedEvent(Id));

        return Result.Updated;
    }

    public Result<Deleted> Delete()
    {
        AddDomainEvent(new AdminDeletedEvent(Id));
        return Result.Deleted;
    }
}
