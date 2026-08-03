using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions;
using QuizNova.Tests.Common.Courses;

namespace QuizNova.Tests.Common.Quizzes;

public static class QuizFactory
{
    public static Result<Quiz> CreateQuiz(
        Guid? id = null,
        Guid? courseId = null,
        Guid? instructorId = null,
        string title = "Test Quiz",
        DateTimeOffset? startsAtUtc = null,
        DateTimeOffset? endsAtUtc = null,
        IEnumerable<CreateQuestionArgs>? questionArgs = null,
        Course? course = null,
        string courseName = "Test Course",
        string instructorName = "Test Instructor")
    {
        var quizId = id ?? Guid.NewGuid();
        questionArgs ??= [
            new CreateTfArgs("Question 1?", 10, true),
            new CreateTfArgs("Question 2?", 10, false),
            new CreateTfArgs("Question 3?", 10, true),
        ];

        if (courseId.HasValue && courseId.Value == Guid.Empty)
        {
            return QuizErrors.CourseIdRequired;
        }

        if (course == null && instructorId.HasValue && instructorId.Value == Guid.Empty)
        {
            return QuizErrors.InstructorIdRequired;
        }

        var resolvedCourse = course ?? CourseFactory.CreateCourse(
            instructorId: instructorId,
            name: courseName,
            maximumMarks: 500).Value;

        return Quiz.Create(
            quizId,
            resolvedCourse.Id,
            instructorId ?? Guid.NewGuid(),
            courseName,
            instructorName,
            title,
            startsAtUtc ?? DateTimeOffset.UtcNow.AddHours(1),
            endsAtUtc ?? DateTimeOffset.UtcNow.AddHours(3),
            questionArgs,
            resolvedCourse);
    }
}
