using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Application.Features.QuizAttempts.Mappers;

public static class QuizAttemptMapper
{
    public static QuizAttemptDto ToQuizAttemptDto(this QuizAttempt quizAttempt)
    {
        var studentAnswers = quizAttempt.StudentAnswers.ToList();
        var answeredQuestions = studentAnswers.Count;

        var questionsById = quizAttempt.Quiz?.Questions
            .ToDictionary(question => question.Id) ?? new Dictionary<Guid, Question>();
        var totalQuestions = questionsById.Count;
        var questionDtos = quizAttempt.Quiz?.Questions
            .OrderBy(question => question.DisplayOrder)
            .Select(question => question.ToQuestionDto())
            .ToList() ?? [];

        var answerDtos = studentAnswers
            .Select(answer => answer.ToDto(questionsById))
            .ToList();

        var correctAnswers = answerDtos.OfType<AutoGradedAnswerDto>().Count(answer => answer.IsCorrect);
        var totalMarks = quizAttempt.Quiz?.Questions.Sum(question => question.Marks) ?? 0;

        return new QuizAttemptDto(
            quizAttempt.Id,
            quizAttempt.QuizId,
            quizAttempt.Quiz?.Title ?? string.Empty,
            quizAttempt.StartedAt,
            quizAttempt.SubmittedAt,
            totalQuestions,
            answeredQuestions,
            correctAnswers,
            quizAttempt.Score,
            totalMarks,
            quizAttempt.Status.ToString(),
            quizAttempt.GradingState.ToString(),
            questionDtos,
            answerDtos);
    }
}
