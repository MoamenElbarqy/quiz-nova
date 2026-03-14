using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.Essay;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.Essay;

public class EssayQuestionAnswer : AuditableEntity
{
    public Guid StudentId { get; private set; }

    public Guid QuestionId { get; private set; }

    public string AnswerText { get; private set; } = string.Empty;

    public bool? IsCorrect { get; private set; }

    public Student? Student { get; private set; }

    public EssayQuestion? Question { get; private set; }

    private EssayQuestionAnswer()
    {
    }

    private EssayQuestionAnswer(Guid id, Guid studentId, Guid questionId, string answerText, bool isCorrect)
        : base(id)
    {
        StudentId = studentId;
        QuestionId = questionId;
        AnswerText = answerText;
        IsCorrect = isCorrect;
    }

    public static Result<EssayQuestionAnswer> Create(
        Guid id,
        Guid studentId,
        Guid questionId,
        string answerText,
        bool isCorrect)
    {
        if (studentId == Guid.Empty)
        {
            return EssayQuestionAnswerErrors.StudentIdRequired;
        }

        if (questionId == Guid.Empty)
        {
            return EssayQuestionAnswerErrors.QuestionIdRequired;
        }

        if (string.IsNullOrWhiteSpace(answerText))
        {
            return EssayQuestionAnswerErrors.AnswerTextRequired;
        }

        return new EssayQuestionAnswer(id, studentId, questionId, answerText, isCorrect);
    }
}
