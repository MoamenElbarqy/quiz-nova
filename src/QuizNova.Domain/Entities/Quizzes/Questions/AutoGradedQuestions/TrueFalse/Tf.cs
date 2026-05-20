using System.Diagnostics.CodeAnalysis;

using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.AutoGradedAnswers.TrueFalseAnswer;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;

namespace QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;

public class Tf : AutoGradedQuestion<bool>
{
    public bool CorrectChoice { get; private set; }

    // Required by Entity Framework Core
    [SetsRequiredMembers]
    private Tf()
    {
    }

    [SetsRequiredMembers]
    private Tf(
        Guid id,
        Guid quizId,
        string questionText,
        bool correctChoice,
        int displayOrder,
        int marks)
        : base(id, quizId, questionText, displayOrder, marks)
    {
        CorrectChoice = correctChoice;
    }

    public static Result<Tf> Create(
        Guid id,
        Guid quizId,
        string questionText,
        bool correctChoice,
        int displayOrder,
        int marks)
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

        return new Tf(
            id,
            quizId,
            questionText,
            correctChoice,
            displayOrder,
            marks);
    }

    internal Result<Updated> Update(
        string questionText,
        int displayOrder,
        int marks,
        bool correctChoice)
    {
        var baseResult = UpdateBase(questionText, displayOrder, marks);

        if (baseResult.IsError)
        {
            return baseResult.TopError;
        }

        CorrectChoice = correctChoice;

        return Result.Updated;
    }

    public override Func<bool, bool> CorrectionCondition => studentChoice => studentChoice == CorrectChoice;

    public override Result<QuestionAnswer> Solve(bool answer, Guid studentId, Guid quizAttemptId)
    {
        var isCorrect = CorrectionCondition(answer);
        return TfAnswer.Create(
            Guid.NewGuid(),
            studentId,
            Id,
            quizAttemptId,
            answer,
            isCorrect).Value;
    }
}
