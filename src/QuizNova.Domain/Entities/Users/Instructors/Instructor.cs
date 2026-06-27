using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users.Instructors;

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
        List<Course> courses,
        List<Quiz> quizzes)
        : base(
            id,
            personalInformation,
            UserRole.Instructor)
    {
        _courses = courses;
        _quizzes = quizzes;
    }

    public IEnumerable<Course> Courses => _courses.AsReadOnly();

    public IEnumerable<Quiz> Quizzes => _quizzes.AsReadOnly();

    public static Result<Instructor> Create(
        Guid id,
        PersonalInformation personalInformation,
        List<Course> courses,
        List<Quiz> quizzes)
    {
        var instructor = new Instructor(
            id,
            personalInformation,
            courses,
            quizzes);
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
