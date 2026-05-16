using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers;

public abstract class AutoGradedAnswer : QuestionAnswer
{
    public bool IsCorrect { get; private set; }

    protected AutoGradedAnswer(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        bool isCorrect)
        : base(id, studentId, questionId, quizAttemptId)
    {
        IsCorrect = isCorrect;
    }

    // private AutoGradedAnswer()
    // {
    // }
}
