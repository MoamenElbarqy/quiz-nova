using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.StudentCourses;
using QuizNova.Domain.Entities.Users.Student.Events;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users.Student;

public class Student : User
{
    private readonly List<QuizAttempt> _quizAttempts;
    private readonly List<StudentCourse> _studentCourses;

    private Student()
    {
    }

    private Student(
        Guid id,
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens,
        List<StudentCourse> studentCourses,
        List<QuizAttempt> quizAttempts)
        : base(
            id,
            personalInformation,
            UserRole.Student,
            refreshTokens)
    {
        _studentCourses = studentCourses;
        _quizAttempts = quizAttempts;
    }

    public IEnumerable<StudentCourse> StudentCourses => _studentCourses.AsReadOnly();
    public IEnumerable<QuizAttempt> QuizAttempts => _quizAttempts.AsReadOnly();

    public static Result<Student> Create(
        Guid id,
        PersonalInformation personalInformation,
        List<RefreshToken> refreshTokens,
        List<StudentCourse> courseEnrollments,
        List<QuizAttempt> quizAttempts)
    {
        var student = new Student(
            id,
            personalInformation,
            refreshTokens,
            courseEnrollments,
            quizAttempts);
        student.AddDomainEvent(new StudentCreatedEvent(id));
        return student;
    }

    public Result<Updated> Update(PersonalInformation personalInformation)
    {
        PersonalInformation = personalInformation;
        AddDomainEvent(new StudentUpdatedEvent(Id));

        return Result.Updated;
    }

    public Result<Deleted> Delete()
    {
        AddDomainEvent(new StudentDeletedEvent(Id));
        return Result.Deleted;
    }
}
