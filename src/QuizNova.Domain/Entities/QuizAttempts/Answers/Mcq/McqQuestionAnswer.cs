using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.Mcq;

public class McqQuestionAnswer : AuditableEntity
{
    public Guid StudentId { get; private set; }

    public Guid QuestionId { get; private set; }

    public Guid SelectedChoiceId { get; private set; }

    public Student? Student { get; private set; }

    public McqQuestion? Question { get; private set; }

    public bool IsCorrect => Question is not null && Question.CorrectChoiceId == SelectedChoiceId;

    private McqQuestionAnswer()
    {
    }

    private McqQuestionAnswer(Guid id, Guid studentId, Guid questionId, Guid selectedChoiceId)
        : base(id)
    {
        StudentId = studentId;
        QuestionId = questionId;
        SelectedChoiceId = selectedChoiceId;
    }

    public static Result<McqQuestionAnswer> Create(Guid id, Guid studentId, Guid questionId, Guid selectedChoiceId)
    {
        if (studentId == Guid.Empty)
        {
            return McqQuestionAnswerErrors.StudentIdRequired;
        }

        if (questionId == Guid.Empty)
        {
            return McqQuestionAnswerErrors.QuestionIdRequired;
        }

        if (selectedChoiceId == Guid.Empty)
        {
            return McqQuestionAnswerErrors.SelectedChoiceIdRequired;
        }

        return new McqQuestionAnswer(id, studentId, questionId, selectedChoiceId);
    }
}
