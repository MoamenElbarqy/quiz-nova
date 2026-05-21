using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Enrollments;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.Users.Student.Events;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users.Student;

public class Student : User
{
    private readonly List<QuizAttempt> _quizAttempts;
    private readonly List<Enrollment> _enrollments;

    private Student()
    {
    }

    private Student(
        Guid id,
        PersonalInformation personalInformation,
        List<Enrollment> enrollments,
        List<QuizAttempt> quizAttempts)
        : base(
            id,
            personalInformation,
            UserRole.Student)
    {
        _enrollments = enrollments;
        _quizAttempts = quizAttempts;
    }

    public IEnumerable<Enrollment> Enrollments => _enrollments.AsReadOnly();
    public IEnumerable<QuizAttempt> QuizAttempts => _quizAttempts.AsReadOnly();

    public static Result<Student> Create(
        Guid id,
        PersonalInformation personalInformation,
        List<Enrollment> courseEnrollments,
        List<QuizAttempt> quizAttempts)
    {
        var student = new Student(
            id,
            personalInformation,
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
