using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Application.Features.QuizAttempts.Mappers;

public static class QuizAttemptMapper
{
    public static QuizAttemptDto ToQuizAttemptDto(this QuizAttempt quizAttempt, Quiz? quiz = null)
    {
        var studentAnswers = quizAttempt.StudentAnswers.ToList();
        var answeredQuestions = studentAnswers.Count;

        var questions = quiz?.Questions.ToList();
        var questionsById = questions
            ?.ToDictionary(question => question.Id) ?? new Dictionary<Guid, Question>();

        var totalQuestions = questionsById.Count;
        var questionDtos = questions
            ?.OrderBy(question => question.DisplayOrder)
            .Select(question => question.ToQuestionDto())
            .ToList() ?? [];

        var answerDtos = studentAnswers
            .Select(answer => answer.ToDto(questionsById))
            .ToList();

        var correctAnswers = answerDtos.OfType<AutoGradedAnswerDto>().Count(answer => answer.IsCorrect);
        var totalMarks = questions?.Sum(question => question.Marks) ?? 0;

        return new QuizAttemptDto(
            quizAttempt.Id,
            quizAttempt.QuizId,
            quiz?.Title ?? string.Empty,
            quizAttempt.StartedAt,
            quizAttempt.SubmittedAt ?? default,
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
