using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Users.Instructors;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;
using QuizNova.Tests.Common.Users.UserPersonalInformation;

namespace QuizNova.Tests.Common.Users.Instructors;

public static class InstructorFactory
{
    public static Result<Instructor> CreateInstructor(
        Guid? id = null,
        PersonalInformation? personalInformation = null,
        List<Course>? courses = null,
        List<Quiz>? quizzes = null)
    {
        return Instructor.Create(
            id ?? Guid.NewGuid(),
            personalInformation ??
            PersonalInformationFactory.CreatePersonalInformation(name: "Test Instructor",
                email: "instructor@example.com"),
            courses ?? [],
            quizzes ?? []);
    }
}
