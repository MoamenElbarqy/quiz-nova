using System.Diagnostics.CodeAnalysis;

using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

public abstract class ManuallyGradedQuestion<TAnswer> : Question<TAnswer>
{
    public int? Score { get; private set; }

    // Required By Ef core
    [SetsRequiredMembers]
    protected ManuallyGradedQuestion()
    {
    }

    [SetsRequiredMembers]
    protected ManuallyGradedQuestion(
        Guid id,
        Guid quizId,
        string questionText,
        int displayOrder,
        int marks)
        : base(id, quizId, questionText, displayOrder, marks)
    {
    }
}
