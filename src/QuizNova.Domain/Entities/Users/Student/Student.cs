using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.StudentCourses;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users.Student;

public class Student : User
{
    private readonly List<QuizAttempt> _quizAttempts;

    private Student()
    {
    }

    private Student(
        Guid id,
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens,
        List<QuizAttempt> quizAttempts)
        : base(
            id,
            personalInformation,
            UserRole.Student,
            refreshTokens)
    {
        _quizAttempts = quizAttempts;
    }

    public IEnumerable<QuizAttempt> QuizAttempts => _quizAttempts.AsReadOnly();

    public static Result<Student> Create(
        Guid id,
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens,
        List<StudentCourse> courseEnrollments,
        List<QuizAttempt> quizAttempts)
    {
        var validationError = ValidateCommon(personalInformation, UserRole.Student);

        if (validationError.IsError)
        {
            return validationError.TopError;
        }

        return new Student(
            id,
            personalInformation,
            refreshTokens,
            quizAttempts);
    }
}
