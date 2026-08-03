using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;

namespace QuizNova.Tests.Common.Courses;

public static class CourseFactory
{
    public static Result<Course> CreateCourse(
        Guid? instructorId = null,
        string name = "Test Course",
        int minimumPassingMarks = 50,
        int maximumMarks = 100)
    {
        return Course.Create(
            instructorId,
            name,
            minimumPassingMarks,
            maximumMarks);
    }
}
