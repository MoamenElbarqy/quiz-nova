using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Essay;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.Essay;

public class EssayQuestionAnswer : QuestionAnswer
{
    public string AnswerText { get; private set; } = string.Empty;

    public bool? IsCorrect { get; private set; }

    public EssayQuestion? Question => base.Question as EssayQuestion;

    // Required by EF Core
    private EssayQuestionAnswer()
        : base(
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            Guid.Empty)
    {
    }

    private EssayQuestionAnswer(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        string answerText,
        bool isCorrect)
        : base(id, studentId, questionId, quizAttemptId)
    {
        AnswerText = answerText;
        IsCorrect = isCorrect;
    }

    public static Result<EssayQuestionAnswer> Create(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        string answerText,
        bool isCorrect)
    {
        var commonValidationError = ValidateCommon(studentId, questionId, quizAttemptId);

        if (commonValidationError.IsError)
        {
            return commonValidationError.TopError;
        }

        if (string.IsNullOrWhiteSpace(answerText))
        {
            return EssayQuestionAnswerErrors.AnswerTextRequired;
        }

        return new EssayQuestionAnswer(
            id,
            studentId,
            questionId,
            quizAttemptId,
            answerText,
            isCorrect);
    }
}
