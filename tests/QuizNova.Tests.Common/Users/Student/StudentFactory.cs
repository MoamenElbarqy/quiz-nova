using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Enrollments;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.Users.Student;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;
using QuizNova.Tests.Common.Users.UserPersonalInformation;

namespace QuizNova.Tests.Common.Users.Students;

public static class StudentFactory
{
    public static Result<Student> CreateStudent(
        PersonalInformation? personalInformation = null,
        List<RefreshToken>? refreshTokens = null,
        List<Enrollment>? courseEnrollments = null,
        List<QuizAttempt>? quizAttempts = null)
    {
        return Student.Create(
            personalInformation ?? PersonalInformationFactory.CreatePersonalInformation(name: "Test Student", email: "student@example.com"),
            refreshTokens ?? [],
            courseEnrollments ?? [],
            quizAttempts ?? []);
    }
}
