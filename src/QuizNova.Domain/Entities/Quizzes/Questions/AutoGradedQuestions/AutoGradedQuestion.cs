using System.Diagnostics.CodeAnalysis;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions;

public abstract class AutoGradedQuestion<TAnswer> : Question
{
    public abstract Func<TAnswer, bool> CorrectionCondition { get; }

    public abstract Result<QuestionAnswer> Solve(TAnswer answer, Guid studentId, Guid quizAttemptId);

    [SetsRequiredMembers]
    protected AutoGradedQuestion()
    {
    }

    [SetsRequiredMembers]
    protected AutoGradedQuestion(
        Guid id,
        Guid quizId,
        string questionText,
        int displayOrder,
        int marks)
        : base(id, quizId, questionText, displayOrder, marks)
    {
    }
}
