using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Quizzes;

namespace QuizNova.Tests.Common.Courses;

public static class CourseFactory
{
    public static Result<Course> CreateCourse(
        Guid? instructorId = null,
        string name = "Test Course",
        int minimumPassingMarks = 50,
        int maximumMarks = 100,
        List<Quiz>? quizzes = null)
    {
        return Course.Create(
            instructorId,
            name,
            minimumPassingMarks,
            maximumMarks,
            quizzes ?? [],
            []);
    }
}
