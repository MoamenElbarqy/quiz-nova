using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers.EssayAnswer;

public class EssayAnswer : ManuallyGradedAnswers
{
    public string StudentResponse { get; private set; }

    private EssayAnswer()
        : base(
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            null,
            0)
    {
        StudentResponse = string.Empty;
    }

    private EssayAnswer(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        string studentResponse,
        int? score,
        int maxMarks)
        : base(id, studentId, questionId, quizAttemptId, score, maxMarks)
    {
        StudentResponse = studentResponse;
    }

    public static Result<EssayAnswer> Create(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        string studentResponse,
        int marks,
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
            score,
            marks);
    }

    public void Update(string studentResponse)
    {
        StudentResponse = studentResponse;
        ResetGrading();
    }
}
