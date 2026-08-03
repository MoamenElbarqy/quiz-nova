using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers;

public abstract class AutoGradedAnswer(
    Guid id,
    Guid studentId,
    Guid questionId,
    Guid quizAttemptId,
    bool isCorrect,
    int marks) : QuestionAnswer(id, studentId, questionId, quizAttemptId)
{
    public bool IsCorrect { get; private set; } = isCorrect;

    public int Marks { get; private set; } = marks;

    protected void UpdateIsCorrect(bool isCorrect)
    {
        IsCorrect = isCorrect;
    }

    protected void UpdateMarks(int marks)
    {
        Marks = marks;
    }
}
