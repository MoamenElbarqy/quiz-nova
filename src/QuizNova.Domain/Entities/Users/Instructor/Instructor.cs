using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Departments;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users;

public class Instructor : User
{
    private readonly List<Course> _courses;

    private readonly List<Department> _departments;

    private readonly List<Quiz> _quizzes;

    private Instructor()
    {
        _courses = new List<Course>();
        _departments = new List<Department>();
        _quizzes = new List<Quiz>();
    }

    private Instructor(
        Guid id,
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens,
        List<Course> courses,
        List<Department> departments,
        List<Quiz> quizzes)
        : base(
            id,
            personalInformation,
            Role.Instructor,
            refreshTokens)
    {
        _courses = courses;
        _departments = departments;
        _quizzes = quizzes;
    }


    public IEnumerable<Course> Courses => _courses.AsReadOnly();

    public IEnumerable<Department> Departments => _departments.AsReadOnly();

    public IEnumerable<Quiz> Quizzes => _quizzes.AsReadOnly();

    public static Result<Instructor> Create(
        Guid id,
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens,
        List<Course> courses,
        List<Department> departments,
        List<Quiz> quizzes)
    {

        var validationError = ValidateCommon(personalInformation, Role.Instructor);

        if (validationError.IsError)
        {
            return validationError.TopError;
        }

        return new Instructor(
            id,
            personalInformation,
            refreshTokens,
            courses,
            departments,
            quizzes);
    }
}
