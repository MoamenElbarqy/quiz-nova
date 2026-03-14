using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.TrueFalse;

public class TrueFalseQuestionAnswer : AuditableEntity
{
    public Guid StudentId { get; private set; }

    public Guid QuestionId { get; private set; }

    public bool StudentChoice { get; private set; }

    public Student? Student { get; private set; }

    public TrueFalseQuestion? Question { get; private set; }

    public bool IsCorrect => Question is not null && Question.CorrectChoice == StudentChoice;

    private TrueFalseQuestionAnswer()
    {
    }

    private TrueFalseQuestionAnswer(Guid id, Guid studentId, Guid questionId, bool studentChoice)
        : base(id)
    {
        StudentId = studentId;
        QuestionId = questionId;
        StudentChoice = studentChoice;
    }

    public static Result<TrueFalseQuestionAnswer> Create(Guid id, Guid studentId, Guid questionId, bool studentChoice)
    {
        if (studentId == Guid.Empty)
        {
            return TrueFalseQuestionAnswerErrors.StudentIdRequired;
        }

        if (questionId == Guid.Empty)
        {
            return TrueFalseQuestionAnswerErrors.QuestionIdRequired;
        }

        return new TrueFalseQuestionAnswer(id, studentId, questionId, studentChoice);
    }
}
