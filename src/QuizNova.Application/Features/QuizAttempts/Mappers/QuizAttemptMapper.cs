using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Essay;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Mcq;
using QuizNova.Domain.Entities.QuizAttempts.Answers.TrueFalse;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

namespace QuizNova.Application.Features.QuizAttempts.Mappers;

public static class QuizAttemptMapper
{
    public static QuizAttemptDto ToQuizAttemptDto(this QuizAttempt quizAttempt)
    {
        var answeredQuestions = quizAttempt.StudentAnswers.Count();

        var questionsById = quizAttempt.Quiz?.Questions
            .ToDictionary(question => question.Id) ?? new Dictionary<Guid, Question>();

        var answerDtos = quizAttempt.StudentAnswers
            .Select(answer => MapAnswer(answer, questionsById))
            .ToList();

        var quizTotalMarks = quizAttempt.Quiz?.Marks ?? 0;

        return new QuizAttemptDto(
            quizAttempt.Id,
            quizAttempt.QuizId,
            quizAttempt.Quiz?.Title ?? string.Empty,
            quizAttempt.StartedAt,
            quizAttempt.SubmittedAt,
            quizAttempt.Quiz?.Questions.Count() ?? 0,
            answeredQuestions,
            answerDtos.Count(answer => answer.IsCorrect == true),
            quizAttempt.SubmittedAt.HasValue,
            answerDtos);
    }

    private static StudentAttemptAnswerDto MapAnswer(
        QuestionAnswer answer,
        IReadOnlyDictionary<Guid, Question> questionsById)
    {
        questionsById.TryGetValue(answer.QuestionId, out var question);

        var questionText = question?.QuestionText ?? string.Empty;

        return answer switch
        {
            McqQuestionAnswer mcqAnswer => new StudentAttemptAnswerDto(
                mcqAnswer.Id,
                mcqAnswer.QuestionId,
                questionText,
                "mcq",
                GetMcqIsCorrect(mcqAnswer, question),
                mcqAnswer.SelectedChoiceId,
                null,
                null),

            TrueFalseQuestionAnswer trueFalseAnswer => new StudentAttemptAnswerDto(
                trueFalseAnswer.Id,
                trueFalseAnswer.QuestionId,
                questionText,
                "true-false",
                GetTrueFalseIsCorrect(trueFalseAnswer, question),
                null,
                trueFalseAnswer.StudentChoice,
                null),

            EssayQuestionAnswer essayAnswer => new StudentAttemptAnswerDto(
                essayAnswer.Id,
                essayAnswer.QuestionId,
                questionText,
                "essay",
                essayAnswer.IsCorrect,
                null,
                null,
                essayAnswer.AnswerText),

            _ => new StudentAttemptAnswerDto(
                answer.Id,
                answer.QuestionId,
                questionText,
                "unknown",
                null,
                null,
                null,
                null),
        };
    }

    private static bool? GetMcqIsCorrect(McqQuestionAnswer answer, Question? question)
    {
        if (question is not McqQuestion mcqQuestion)
        {
            return null;
        }

        return answer.SelectedChoiceId == mcqQuestion.CorrectChoiceId;
    }

    private static bool? GetTrueFalseIsCorrect(TrueFalseQuestionAnswer answer, Question? question)
    {
        if (question is not TrueFalseQuestion trueFalseQuestion)
        {
            return null;
        }

        return answer.StudentChoice == trueFalseQuestion.CorrectChoice;
    }
}
