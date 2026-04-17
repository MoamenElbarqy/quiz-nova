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
        ArgumentNullException.ThrowIfNull(quizAttempt);

        var studentAnswers = quizAttempt.StudentAnswers.ToList();
        var answeredQuestions = studentAnswers.Count;

        var questionsById = quizAttempt.Quiz?.Questions
            .ToDictionary(question => question.Id) ?? new Dictionary<Guid, Question>();
        var totalQuestions = questionsById.Count;

        var answerDtos = studentAnswers
            .Select(answer => MapAnswer(answer, questionsById))
            .ToList();

        var quizTotalMarks = quizAttempt.Quiz?.Marks ?? 0;
        var correctAnswers = answerDtos.Count(answer => answer.IsCorrect == true);
        var score = studentAnswers.Sum(answer => GetEarnedMarks(answer, questionsById));
        var isSubmitted = quizAttempt.SubmittedAt.HasValue;
        var isPassed = isSubmitted && quizTotalMarks > 0 && score == quizTotalMarks;

        return new QuizAttemptDto(
            quizAttempt.Id,
            quizAttempt.QuizId,
            quizAttempt.Quiz?.Title ?? string.Empty,
            quizAttempt.StartedAt,
            quizAttempt.SubmittedAt,
            totalQuestions,
            answeredQuestions,
            correctAnswers,
            score,
            answerDtos,
            isSubmitted,
            isPassed);
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

    private static int GetEarnedMarks(QuestionAnswer answer, IReadOnlyDictionary<Guid, Question> questionsById)
    {
        questionsById.TryGetValue(answer.QuestionId, out var question);

        return answer switch
        {
            McqQuestionAnswer mcqAnswer when question is McqQuestion mcqQuestion &&
                                            mcqAnswer.SelectedChoiceId == mcqQuestion.CorrectChoiceId
                => mcqQuestion.Marks,

            TrueFalseQuestionAnswer trueFalseAnswer when question is TrueFalseQuestion trueFalseQuestion &&
                                                  trueFalseAnswer.StudentChoice == trueFalseQuestion.CorrectChoice
                => trueFalseQuestion.Marks,

            EssayQuestionAnswer { IsCorrect: true } when question is not null
                => question.Marks,

            _ => 0,
        };
    }
}
