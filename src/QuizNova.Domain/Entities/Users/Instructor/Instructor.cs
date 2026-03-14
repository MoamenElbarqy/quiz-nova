using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities..Courses;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Domain.Entities.Users;

public class Instructor : User
{
    public List<Course> Courses { get; private set; } = [];

    public List<Quiz> Quizzes { get; private set; } = [];

    private Instructor()
    {
    }

    private Instructor(User user)
        : base(user.Id,
               user.Name,
               user.Email,
               user.Password,
               user.PhoneNumber,
               Role.Instructor)
    {
    }

    public static Result<Instructor> Create(Guid id, string name, string email, string password, string phoneNumber)
    {
        var user = User.Create(id,
                               name,
                               email,
                               password,
                               phoneNumber,
                               Role.Instructor);

        if (user.IsError)
        {
            return user.Errors;
        }

        return new Instructor(user.Value);
    }
}
