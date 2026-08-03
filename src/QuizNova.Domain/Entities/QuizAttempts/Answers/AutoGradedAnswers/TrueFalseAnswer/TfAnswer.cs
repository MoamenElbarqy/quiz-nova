using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers.TrueFalseAnswer;

public class TfAnswer : AutoGradedAnswer
{
    public bool StudentChoice { get; private set; }

    public void Update(bool studentChoice, bool isCorrect)
    {
        StudentChoice = studentChoice;
        UpdateIsCorrect(isCorrect);
    }

    private TfAnswer()
        : base(
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            false,
            0)
    {
    }

    private TfAnswer(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        bool studentChoice,
        bool isCorrect,
        int marks)
        : base(id, studentId, questionId, quizAttemptId, isCorrect, marks)
    {
        StudentChoice = studentChoice;
    }

    public static Result<TfAnswer> Create(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        bool studentChoice,
        bool isCorrect,
        int marks)
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
            isCorrect,
            marks);
    }
}
