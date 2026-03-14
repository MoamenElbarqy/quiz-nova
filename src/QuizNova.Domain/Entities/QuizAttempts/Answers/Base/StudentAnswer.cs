using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq.Choices;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

public class StudentAnswer : AuditableEntity
{
    public Guid StudentId { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid QuizAttemptId { get; private set; }
    public Guid SelectedChoiceId { get; private set; }
    public Choice? SelectedChoice { get; private set; }

    public bool IsCorrect => Question is McqQuestion mcq && SelectedChoiceId == mcq.CorrectChoiceId;
    public QuizAttempt? QuizAttempt { get; private set; }
    public Student? Student { get; private set; }
    public Question? Question { get; private set; }

    private StudentAnswer()
    {
    }

    private StudentAnswer(Guid id,
                          Guid studentId,
                          Guid questionId,
                          Guid quizAttemptId,
                          Guid selectedChoiceId)
        : base(id)
    {
        StudentId = studentId;
        QuestionId = questionId;
        QuizAttemptId = quizAttemptId;
        SelectedChoiceId = selectedChoiceId;
    }

    public static Result<StudentAnswer> Create(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        Guid selectedChoiceId)
    {
        if (studentId == Guid.Empty)
        {
            return StudentAnswerErrors.StudentIdRequired;
        }

        if (questionId == Guid.Empty)
        {
            return StudentAnswerErrors.QuestionIdRequired;
        }

        if (quizAttemptId == Guid.Empty)
        {
            return StudentAnswerErrors.QuizAttemptIdRequired;
        }

        if (selectedChoiceId == Guid.Empty)
        {
            return StudentAnswerErrors.SelectedChoiceIdRequired;
        }

        return new StudentAnswer(id, studentId, questionId, quizAttemptId, selectedChoiceId);
    }
}
