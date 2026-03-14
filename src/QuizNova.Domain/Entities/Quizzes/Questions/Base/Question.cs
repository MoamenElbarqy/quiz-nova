using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Base;

public abstract class Question : AuditableEntity
{
    public Guid QuizId { get; protected set; }

    public int DisplayOrder { get; protected set; }

    public int Points { get; protected set; }

    public Quiz? Quiz { get; protected set; }

    protected Question()
    {
    }

    protected Question(Guid id, Guid quizId, int displayOrder, int points)
        : base(id)
    {
        QuizId = quizId;
        DisplayOrder = displayOrder;
        Points = points;
    }
}