using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Entities.Quizzes;

namespace QuizNova.Application.Features.Quizzes.Mappers;

public static class QuizMapper
{
    public static QuizDto ToQuizDto(this Quiz quiz, string courseName, string instructorName)
    {
        var now = DateTimeOffset.UtcNow;
        return new QuizDto
        {
            QuizId = quiz.Id,
            Title = quiz.Title,
            CourseName = courseName,
            InstructorName = instructorName,
            Marks = quiz.Questions.Sum(question => question.Marks),
            ServerUtc = now,
            State = quiz.StartsAtUtc > now ? "Upcoming" : quiz.EndsAtUtc < now ? "Completed" : "Active",
            CourseId = quiz.CourseId,
            InstructorId = quiz.InstructorId,
            StartsAtUtc = quiz.StartsAtUtc,
            EndsAtUtc = quiz.EndsAtUtc,
            Questions = quiz.Questions.Select(q => q.ToQuestionDto()).ToArray(),
        };
    }
}
