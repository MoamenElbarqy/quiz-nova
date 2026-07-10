using System.Diagnostics.CodeAnalysis;

using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers.McqAnswer;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;

namespace QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;

public class Mcq : AutoGradedQuestion<Guid>
{
    private readonly List<Choice> _choices;

    [SetsRequiredMembers]
    private Mcq()
    {
    }

    [SetsRequiredMembers]
    private Mcq(
        Guid id,
        Guid quizId,
        string questionText,
        Guid? correctChoiceId,
        int displayOrder,
        int marks,
        List<Choice> choices)
        : base(id, quizId, questionText, displayOrder, marks)
    {
        CorrectChoiceId = correctChoiceId;
        _choices = choices;
    }

    public int NumberOfChoices => Choices.Count();

    public Guid? CorrectChoiceId { get; private set; }

    public Choice? CorrectChoice { get; init; }

    public IEnumerable<Choice> Choices => _choices.AsReadOnly();

    public static Result<Mcq> Create(
        Guid id,
        Guid quizId,
        string questionText,
        Guid correctChoiceId,
        int displayOrder,
        int marks,
        List<Choice> choices)
    {
        var validationError = ValidateCommon(
            quizId,
            questionText,
            displayOrder,
            marks);

        if (validationError.IsError)
        {
            return validationError.TopError;
        }

        var trimmedText = questionText.Trim();

        if (trimmedText.Length < 3)
        {
            return McqErrors.TitleTooShort;
        }

        if (trimmedText.Length > 500)
        {
            return McqErrors.TitleTooLong;
        }

        if (choices.Count < 2)
        {
            return McqErrors.NumberOfChoicesInvalid;
        }

        if (correctChoiceId == Guid.Empty)
        {
            return McqErrors.CorrectChoiceIdRequired;
        }

        return new Mcq(
            id,
            quizId,
            questionText,
            correctChoiceId,
            displayOrder,
            marks,
            choices);
    }

    internal Result<Updated> Update(
        string questionText,
        int displayOrder,
        int marks,
        Guid correctChoiceId,
        List<Choice> choices)
    {
        var baseResult = UpdateBase(questionText, displayOrder, marks);

        if (baseResult.IsError)
        {
            return baseResult.TopError;
        }

        var trimmedText = questionText.Trim();

        if (trimmedText.Length < 3)
        {
            return McqErrors.TitleTooShort;
        }

        if (trimmedText.Length > 500)
        {
            return McqErrors.TitleTooLong;
        }

        if (choices.Count < 2)
        {
            return McqErrors.NumberOfChoicesInvalid;
        }

        if (correctChoiceId == Guid.Empty)
        {
            return McqErrors.CorrectChoiceIdRequired;
        }

        if (choices.All(c => c.Id != correctChoiceId))
        {
            return McqErrors.CorrectChoiceIdRequired;
        }

        CorrectChoiceId = correctChoiceId;
        _choices.Clear();
        _choices.AddRange(choices);

        return Result.Updated;
    }

    public override Func<Guid, bool> CorrectionCondition => studentChoiceId => studentChoiceId == CorrectChoiceId;

    public override Result<QuestionAnswer> Solve(Guid answer, Guid studentId, Guid quizAttemptId)
    {
        var isCorrect = CorrectionCondition(answer);
        var createResult = McqAnswer.Create(
            Guid.NewGuid(),
            studentId,
            Id,
            quizAttemptId,
            answer,
            this,
            isCorrect);

        if (createResult.IsError)
        {
            return createResult.TopError;
        }

        return createResult.Value;
    }
}
