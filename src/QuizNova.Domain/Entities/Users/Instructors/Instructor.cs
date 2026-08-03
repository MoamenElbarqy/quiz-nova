using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users.Instructors;

public class Instructor : User
{
    private Instructor()
    {
    }

    private Instructor(
        Guid id,
        PersonalInformation personalInformation)
        : base(
            id,
            personalInformation,
            UserRole.Instructor)
    {
    }

    public static Result<Instructor> Create(
        Guid id,
        PersonalInformation personalInformation)
    {
        var instructor = new Instructor(
            id,
            personalInformation);
        return instructor;
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
