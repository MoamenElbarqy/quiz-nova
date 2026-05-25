using QuizNova.Domain.Common;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.ManuallyGradedAnswers.Events;

public class QuestionGradedEvent(Guid answerId) : DomainEvent
{
    public Guid AnswerId { get; } = answerId;
}
