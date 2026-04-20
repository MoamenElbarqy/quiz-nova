using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.Mcq;

public class McqAnswer : QuestionAnswer
{
    public Guid SelectedChoiceId { get; private set; }

    public Quizzes.Questions.Mcq.Mcq? Question => base.Question as Quizzes.Questions.Mcq.Mcq;

    public bool IsCorrect => Question is not null && Question.CorrectChoiceId == SelectedChoiceId;

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
        Guid selectedChoiceId)
        : base(id, studentId, questionId, quizAttemptId)
    {
        SelectedChoiceId = selectedChoiceId;
    }

    public static Result<McqAnswer> Create(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        Guid selectedChoiceId,
        Quizzes.Questions.Mcq.Mcq question)
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

        return new McqAnswer(id, studentId, questionId, quizAttemptId, selectedChoiceId);
    }
}
