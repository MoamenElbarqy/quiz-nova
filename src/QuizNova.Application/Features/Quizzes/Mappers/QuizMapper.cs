using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Entities.Quizzes;

public static class QuizMapper
{
    public static QuizDetailsDto ToQuizDetailsDto(this Quiz quiz)
    {
        return new QuizDetailsDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            CourseId = quiz.CourseId,
            InstructorId = quiz.InstructorId,
            StartsAtUtc = quiz.StartsAtUtc,
            EndsAtUtc = quiz.EndsAtUtc,
            Questions = quiz.Questions.Select(q => q.ToQuestionDto()).ToArray(),
        };
    }
}
