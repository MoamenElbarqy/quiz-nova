using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;

namespace QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers.McqAnswer;

public class McqAnswer : AutoGradedAnswer
{
    public Guid SelectedChoiceId { get; private set; }

    public Mcq? Mcq { get; init; }

    public void Update(Guid selectedChoiceId, bool isCorrect)
    {
        SelectedChoiceId = selectedChoiceId;
        UpdateIsCorrect(isCorrect);
    }

    // Required by EF Core
    private McqAnswer()
        : base(
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            Guid.Empty,
            false)
    {
    }

    private McqAnswer(
        Guid id,
        Guid studentId,
        Guid questionId,
        Guid quizAttemptId,
        Guid selectedChoiceId,
        Mcq mcq,
        bool isCorrect)
        : base(id, studentId, questionId, quizAttemptId, isCorrect)
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
        Mcq question,
        bool isCorrect)
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

        return new McqAnswer(id, studentId, questionId, quizAttemptId, selectedChoiceId, question, isCorrect);
    }
}
