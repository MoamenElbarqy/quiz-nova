using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users.Admins;

public class Admin : User
{
    private Admin()
    {
    }

    private Admin(
        Guid id,
        PersonalInformation personalInformation)
        : base(
            id,
            personalInformation,
            UserRole.Admin)
    {
    }

    public static Result<Admin> Create(
        Guid id,
        PersonalInformation personalInformation)
    {
        var admin = new Admin(id, personalInformation);
        return admin;
    }

    public Result<Updated> Update(PersonalInformation personalInformation)
    {
        PersonalInformation = personalInformation;

        return Result.Updated;
    }

    public Result<Deleted> Delete()
    {
        return Result.Deleted;
    }
}
