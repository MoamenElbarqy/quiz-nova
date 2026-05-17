using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.TrueFalseAnswer;

public class TfAnswer : AutoGradedAnswer
{
    public bool StudentChoice { get; }

    public Tf? Tf { get; init; }

    // Required by EF Core
    private TfAnswer()
        : base(
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            false)
    {
    }

    private TfAnswer(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        bool studentChoice,
        bool isCorrect)
        : base(id, studentId, questionId, quizAttemptId, isCorrect)
    {
        StudentChoice = studentChoice;
    }

    public static Result<TfAnswer> Create(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        bool studentChoice,
        bool isCorrect)
    {
        var commonValidationError = ValidateCommon(studentId, questionId, quizAttemptId);

        if (commonValidationError.IsError)
        {
            return commonValidationError.TopError;
        }

        return new TfAnswer(
            id,
            studentId: studentId,
            questionId,
            quizAttemptId,
            studentChoice,
            isCorrect);
    }
}
