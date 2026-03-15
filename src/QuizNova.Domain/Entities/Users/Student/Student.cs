using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Colleges;
using QuizNova.Domain.Entities.Departments;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Levels;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.StudentCourses;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.Entities.Users;

public class Student : User
{
    private readonly List<StudentCourse> _courseEnrollments;

    private readonly List<QuizAttempt> _quizAttempts;

    private Student(
        Guid id,
        PersonalInformation personalInformation,
        Guid collegeId,
        Guid departmentId,
        Guid levelId,
        List<RefreshToken> refreshTokens,
        List<StudentCourse> courseEnrollments,
        List<QuizAttempt> quizAttempts)
        : base(
            id,
            personalInformation,
            Role.Student,
            refreshTokens)
    {
        CollegeId = collegeId;
        DepartmentId = departmentId;
        LevelId = levelId;
        _courseEnrollments = courseEnrollments;
        _quizAttempts = quizAttempts;
    }

    public Guid CollegeId { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid LevelId { get; private set; }

    public College? College { get; private set; }

    public Department? Department { get; private set; }

    public Level? Level { get; private set; }

    public IEnumerable<StudentCourse> CourseEnrollments => _courseEnrollments.AsReadOnly();

    public IEnumerable<QuizAttempt> QuizAttempts => _quizAttempts.AsReadOnly();

    public static Result<Student> Create(
        Guid id,
        PersonalInformation personalInformation,
        Guid collegeId,
        Guid departmentId,
        Guid levelId,
        List<RefreshToken> refreshTokens,
        List<StudentCourse> courseEnrollments,
        List<QuizAttempt> quizAttempts)
    {
        if (collegeId == Guid.Empty)
        {
            return StudentErrors.CollegeIdRequired;
        }

        if (departmentId == Guid.Empty)
        {
            return StudentErrors.DepartmentIdRequired;
        }

        if (levelId == Guid.Empty)
        {
            return StudentErrors.LevelIdRequired;
        }

        var validationError = ValidateCommon(personalInformation, Role.Student);

        if (validationError.IsError)
        {
            return validationError.TopError;
        }

        return new Student(
            id,
            personalInformation,
            collegeId,
            departmentId,
            levelId,
            refreshTokens,
            courseEnrollments,
            quizAttempts);
    }
}
