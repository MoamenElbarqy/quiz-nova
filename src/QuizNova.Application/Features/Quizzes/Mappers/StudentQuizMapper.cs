using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Entities.Quizzes;

namespace QuizNova.Application.Features.Quizzes.Mappers;

public static class StudentQuizMapper
{
    public static StudentQuizDto ToStudentQuizDto(this Quiz quiz)
    {
        return new StudentQuizDto(
            quiz.Id,
            quiz.Title,
            quiz.Course?.Name ?? string.Empty,
            quiz.Questions.Count(),
            quiz.StartsAtUtc,
            quiz.EndsAtUtc,
            quiz.Status);
    }
}
