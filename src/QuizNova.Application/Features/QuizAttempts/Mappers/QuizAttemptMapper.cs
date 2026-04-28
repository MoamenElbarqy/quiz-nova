using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Answers.McqAnswer;
using QuizNova.Domain.Entities.QuizAttempts.Answers.TrueFalseAnswer;
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
            .Select(answer => MapAnswer(answer, questionsById))
            .ToList();

        var correctAnswers = answerDtos.Count(answer => answer.IsCorrect);
        var isPassed = quizAttempt.Quiz != null && quizAttempt.Score >= Math.Ceiling(quizAttempt.Quiz.Marks * 0.5);

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
            questionDtos,
            answerDtos,
            isPassed);
    }

    private static QuestionAnswerDto MapAnswer(
        QuestionAnswer answer,
        IReadOnlyDictionary<Guid, Question> questionsById)
    {
        questionsById.TryGetValue(answer.QuestionId, out var question);

        var questionText = question?.QuestionText ?? string.Empty;

        return answer switch
        {
            McqAnswer mcqAnswer => new McqAnswerDto(
                mcqAnswer.Id,
                mcqAnswer.QuestionId,
                questionText,
                "mcq",
                answer.IsCorrect,
                mcqAnswer.SelectedChoiceId),

            TfAnswer tfAnswer => new TfAnswerDto(
                tfAnswer.Id,
                tfAnswer.QuestionId,
                questionText,
                "tf",
                answer.IsCorrect,
                tfAnswer.StudentChoice),

            _ => throw new InvalidOperationException($"Unknown answer type: {answer.GetType().Name}")
        };
    }
}
