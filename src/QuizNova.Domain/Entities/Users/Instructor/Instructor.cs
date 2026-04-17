using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users;

public class Instructor : User
{
    private readonly List<Course> _courses;

    private readonly List<Quiz> _quizzes;

    private Instructor()
    {
    }

    private Instructor(
        Guid id,
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens,
        List<Course> courses,
        List<Quiz> quizzes)
        : base(
            id,
            personalInformation,
            UserRole.Instructor,
            refreshTokens)
    {
        _courses = courses;
        _quizzes = quizzes;
    }

    public IEnumerable<Course> Courses => _courses.AsReadOnly();

    public IEnumerable<Quiz> Quizzes => _quizzes.AsReadOnly();

    public static Result<Instructor> Create(
        Guid id,
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens,
        List<Course> courses,
        List<Quiz> quizzes)
    {
        var validationError = ValidateCommon(personalInformation, UserRole.Instructor);

        if (validationError.IsError)
        {
            return validationError.TopError;
        }

        return new Instructor(
            id,
            personalInformation,
            refreshTokens,
            courses,
            quizzes);
    }
}
