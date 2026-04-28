using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.McqAnswer;

public class McqAnswer : QuestionAnswer
{
    public Guid SelectedChoiceId { get; }

    public Mcq? Mcq { get; init; }

    public override bool IsCorrect => Mcq is not null && SelectedChoiceId == Mcq.CorrectChoiceId;

    // Required by EF Core
    private McqAnswer()
        : base(
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            Guid.Empty)
    {
    }

    private McqAnswer(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        Guid selectedChoiceId,
        Mcq mcq)
        : base(id, studentId, questionId, quizAttemptId)
    {
        SelectedChoiceId = selectedChoiceId;
        Mcq = mcq;
    }

    public static Result<McqAnswer> Create(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        Guid selectedChoiceId,
        Mcq question)
    {
        var commonValidationError = ValidateCommon(studentId, questionId, quizAttemptId);

        if (commonValidationError.IsError)
        {
            return commonValidationError.TopError;
        }

        if (selectedChoiceId == Guid.Empty)
        {
            return McqAnswerErrors.SelectedChoiceIdRequired;
        }

        if (question.Id != questionId)
        {
            return McqAnswerErrors.QuestionMismatch(questionId, question.Id);
        }

        if (question.Choices.All(choice => choice.Id != selectedChoiceId))
        {
            return McqAnswerErrors.SelectedChoiceDoesNotBelongToQuestion(questionId, selectedChoiceId);
        }

        return new McqAnswer(id, studentId, questionId, quizAttemptId, selectedChoiceId, question);
    }
}
