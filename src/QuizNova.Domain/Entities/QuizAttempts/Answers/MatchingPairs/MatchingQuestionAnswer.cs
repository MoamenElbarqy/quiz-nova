using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.MatchingPairs;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.MatchingPairs;

public class MatchingQuestionAnswer : AuditableEntity
{
    public Guid StudentId { get; private set; }

    public Guid QuestionId { get; private set; }

    public Guid LeftChoiceId { get; private set; }

    public Guid RightChoiceId { get; private set; }

    public Student? Student { get; private set; }

    public MatchingQuestion? Question { get; private set; }

    private MatchingQuestionAnswer()
    {
    }

    private MatchingQuestionAnswer(Guid id, Guid studentId, Guid questionId, Guid leftChoiceId, Guid rightChoiceId)
        : base(id)
    {
        StudentId = studentId;
        QuestionId = questionId;
        LeftChoiceId = leftChoiceId;
        RightChoiceId = rightChoiceId;
    }

    public static Result<MatchingQuestionAnswer> Create(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid leftChoiceId,
        Guid rightChoiceId)
    {
        if (studentId == Guid.Empty)
        {
            return MatchingQuestionAnswerErrors.StudentIdRequired;
        }

        if (questionId == Guid.Empty)
        {
            return MatchingQuestionAnswerErrors.QuestionIdRequired;
        }

        if (leftChoiceId == Guid.Empty)
        {
            return MatchingQuestionAnswerErrors.LeftChoiceIdRequired;
        }

        if (rightChoiceId == Guid.Empty)
        {
            return MatchingQuestionAnswerErrors.RightChoiceIdRequired;
        }

        return new MatchingQuestionAnswer(id, studentId, questionId, leftChoiceId, rightChoiceId);
    }
}
