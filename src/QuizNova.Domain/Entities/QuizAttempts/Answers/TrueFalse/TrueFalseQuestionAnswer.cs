using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.TrueFalse;

public class TrueFalseQuestionAnswer : QuestionAnswer
{
    public bool StudentChoice { get; private set; }

    public TrueFalseQuestion? Question => base.Question as TrueFalseQuestion;

    public bool IsCorrect => Question is not null && Question.CorrectChoice == StudentChoice;

    // Required by EF Core
    private TrueFalseQuestionAnswer()
        : base(
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty)
    {
    }

    private TrueFalseQuestionAnswer(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        bool studentChoice)
        : base(id, studentId, questionId, quizAttemptId)
    {
        StudentChoice = studentChoice;
    }

    public static Result<TrueFalseQuestionAnswer> Create(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        bool studentChoice)
    {
        var commonValidationError = ValidateCommon(studentId, questionId, quizAttemptId);

        if (commonValidationError.IsError)
        {
            return commonValidationError.TopError;
        }

        return new TrueFalseQuestionAnswer(
            id,
            studentId: studentId,
            questionId,
            quizAttemptId,
            studentChoice);
    }
}
