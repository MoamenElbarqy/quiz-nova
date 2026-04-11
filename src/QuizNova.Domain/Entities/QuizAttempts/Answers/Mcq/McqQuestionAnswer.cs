using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.Mcq;

public class McqQuestionAnswer : QuestionAnswer
{
    public Guid SelectedChoiceId { get; private set; }

    public McqQuestion? Question => base.Question as McqQuestion;

    public bool IsCorrect => Question is not null && Question.CorrectChoiceId == SelectedChoiceId;

    // Required by EF Core
    private McqQuestionAnswer()
        : base(
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty)
    {
    }

    private McqQuestionAnswer(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        Guid selectedChoiceId)
        : base(id, studentId, questionId, quizAttemptId)
    {
        SelectedChoiceId = selectedChoiceId;
    }

    public static Result<McqQuestionAnswer> Create(Guid id, Guid studentId, Guid questionId, Guid quizAttemptId, Guid selectedChoiceId)
    {
        var commonValidationError = ValidateCommon(studentId, questionId, quizAttemptId);

        if (commonValidationError.IsError)
        {
            return commonValidationError.TopError;
        }

        if (selectedChoiceId == Guid.Empty)
        {
            return McqQuestionAnswerErrors.SelectedChoiceIdRequired;
        }

        return new McqQuestionAnswer(id, studentId, questionId, quizAttemptId, selectedChoiceId);
    }
}
