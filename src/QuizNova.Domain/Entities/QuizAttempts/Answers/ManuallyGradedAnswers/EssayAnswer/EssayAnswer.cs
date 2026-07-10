using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers.EssayAnswer;

public class EssayAnswer : ManuallyGradedAnswers
{
    public string StudentResponse { get; private set; }

    // Required by EF Core
    private EssayAnswer()
        : base(
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            null)
    {
        StudentResponse = string.Empty;
    }

    private EssayAnswer(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        string studentResponse,
        int? score)
        : base(id, studentId, questionId, quizAttemptId, score)
    {
        StudentResponse = studentResponse;
    }

    public static Result<EssayAnswer> Create(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        string studentResponse,
        int? score = null)
    {
        var commonValidationError = ValidateCommon(studentId, questionId, quizAttemptId);

        if (commonValidationError.IsError)
        {
            return commonValidationError.TopError;
        }

        var trimmedResponse = studentResponse.Trim();

        if (string.IsNullOrWhiteSpace(trimmedResponse))
        {
            return EssayAnswerErrors.ResponseRequired;
        }

        if (trimmedResponse.Length < 3)
        {
            return EssayAnswerErrors.ResponseTooShort;
        }

        if (trimmedResponse.Length > 1000)
        {
            return EssayAnswerErrors.ResponseTooLong;
        }

        return new EssayAnswer(
            id,
            studentId,
            questionId,
            quizAttemptId,
            trimmedResponse,
            score);
    }

    public void Update(string studentResponse)
    {
        StudentResponse = studentResponse;
        ResetGrading();
    }
}
