using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users.Student;

public class Student : User
{
    private Student()
    {
    }

    private Student(
        Guid id,
        PersonalInformation personalInformation)
        : base(
            id,
            personalInformation,
            UserRole.Student)
    {
    }

    public static Result<Student> Create(
        Guid id,
        PersonalInformation personalInformation)
    {
        var student = new Student(
            id,
            personalInformation);
        return student;
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
