using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities..Colleges;
using QuizNova.Domain.Entities..Departments;
using QuizNova.Domain.Entities..Levels;
using QuizNova.Domain.Entities..StudentCourses;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Domain.Entities.Users;

public class Student : User
{
    public Guid CollegeId { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid LevelId { get; private set; }

    public College? College { get; private set; }

    public Department? Department { get; private set; }

    public Level? Level { get; private set; }

    public List<StudentCourse> CourseEnrollments { get; private set; } = [];

    public List<QuizAttempts.QuizAttempt> QuizAttempts { get; private set; } = [];

    private Student()
    {
    }

    private Student(User user, Guid collegeId, Guid departmentId, Guid levelId)
        : base(user.Id,
               user.Name,
               user.Email,
               user.Password,
               user.PhoneNumber,
               Role.Student)
    {
        CollegeId = collegeId;
        DepartmentId = departmentId;
        LevelId = levelId;
    }

    public static Result<Student> Create(
        Guid id,
        string name,
        string email,
        string password,
        string phoneNumber,
        Guid collegeId,
        Guid departmentId,
        Guid levelId)
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

        var user = User.Create(id, name, email, password, phoneNumber, Role.Student);

        if (user.IsError)
        {
            return user.Errors;
        }

        return new Student(user.Value, collegeId, departmentId, levelId);
    }
}
