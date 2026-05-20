using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users.Instructors;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;
using QuizNova.Tests.Common.Users.UserPersonalInformation;

namespace QuizNova.Tests.Common.Users.Instructors;

public static class InstructorFactory
{
    public static Result<Instructor> CreateInstructor(
        PersonalInformation? personalInformation = null,
        List<RefreshToken>? refreshTokens = null,
        List<Course>? courses = null,
        List<Quiz>? quizzes = null)
    {
        return Instructor.Create(
            personalInformation ?? PersonalInformationFactory.CreatePersonalInformation(name: "Test Instructor", email: "instructor@example.com"),
            refreshTokens ?? [],
            courses ?? [],
            quizzes ?? []);
    }
}
