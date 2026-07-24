using System.Diagnostics.CodeAnalysis;

using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;
using QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

namespace QuizNova.Domain.Entities.Quizzes.Questions.Base;

public abstract class Question : Entity
{
    [SetsRequiredMembers]
    protected Question()
    {
    }

    [SetsRequiredMembers]
    protected Question(
        Guid id,
        Guid quizId,
        string questionText,
        int displayOrder,
        int marks)
        : base(id)
    {
        QuizId = quizId;
        QuestionText = questionText;
        DisplayOrder = displayOrder;
        Marks = marks;
    }

    public Guid QuizId { get; private set; }

    public required string QuestionText { get; set; }

    public int DisplayOrder { get; private set; }

    public int Marks { get; private set; }

    public Quiz? Quiz { get; init; }

    public static Result<Question> CreateFromArgs(
        CreateQuestionArgs args,
        int displayOrder,
        Guid quizId)
    {
        return args switch
        {
            CreateTfArgs tf => CreateTf(tf, displayOrder, quizId),
            CreateMcqArgs mcq => CreateMcq(mcq, displayOrder, quizId),
            CreateEssayArgs essay => CreateEssay(essay, displayOrder, quizId),
            _ => Error.Unexpected(
                "Quiz.Question.Unsupported",
                $"Unsupported question type '{args.GetType().Name}'."),
        };
    }

    internal Result<Updated> UpdateBase(
        string questionText,
        int displayOrder,
        int marks)
    {
        var validation = ValidateCommon(QuizId, questionText, displayOrder, marks);

        if (validation.IsError)
        {
            return validation.TopError;
        }

        QuestionText = questionText;
        DisplayOrder = displayOrder;
        Marks = marks;

        return Result.Updated;
    }

    protected static Result<Validated> ValidateCommon(
        Guid quizId,
        string questionText,
        int displayOrder,
        int marks)
    {
        if (quizId == Guid.Empty)
        {
            return QuestionErrors.QuizIdRequired;
        }

        if (string.IsNullOrWhiteSpace(questionText))
        {
            return QuestionErrors.QuestionTextRequired;
        }

        if (displayOrder < 0)
        {
            return QuestionErrors.DisplayOrderInvalid;
        }

        if (marks <= 0)
        {
            return QuestionErrors.MarksInvalid;
        }

        return Result.Validated;
    }

    private static Result<Question> CreateTf(CreateTfArgs command, int displayOrder, Guid quizId)
    {
        var questionId = Guid.NewGuid();
        var result = Tf.Create(
            questionId,
            quizId,
            command.QuestionText,
            command.CorrectChoice,
            displayOrder,
            command.Marks);

        return result.IsError ? result.TopError : result.Value;
    }

    private static Result<Question> CreateMcq(CreateMcqArgs command, int displayOrder, Guid quizId)
    {
        var questionId = Guid.NewGuid();

        if (command.Choices.All(choice => choice.Id != command.CorrectChoiceId))
        {
            return McqErrors.CorrectChoiceNotFound(questionId, command.CorrectChoiceId);
        }

        if (command.Choices.GroupBy(choice => choice.Id).Any(group => group.Count() > 1))
        {
            return McqErrors.ChoiceIdsMustBeUnique(questionId);
        }

        var choices = new List<Choice>(command.Choices.Count);
        var actualCorrectChoiceId = Guid.Empty;

        foreach (var choiceCommand in command.Choices)
        {
            var choiceId = Guid.NewGuid();
            if (choiceCommand.Id == command.CorrectChoiceId)
            {
                actualCorrectChoiceId = choiceId;
            }

            var createChoiceResult = Choice.Create(
                choiceId,
                questionId,
                choiceCommand.Text,
                choiceCommand.DisplayOrder);

            if (createChoiceResult.IsError)
            {
                return createChoiceResult.TopError;
            }

            choices.Add(createChoiceResult.Value);
        }

        var createQuestionResult = Mcq.Create(
            questionId,
            quizId,
            command.QuestionText,
            actualCorrectChoiceId,
            displayOrder,
            command.Marks,
            choices);

        return createQuestionResult.IsError ? createQuestionResult.TopError : createQuestionResult.Value;
    }

    private static Result<Question> CreateEssay(CreateEssayArgs command, int displayOrder, Guid quizId)
    {
        var questionId = Guid.NewGuid();
        var result = Essay.Create(
            questionId,
            quizId,
            command.QuestionText,
            command.AnswerReference,
            displayOrder,
            command.Marks);

        return result.IsError ? result.TopError : result.Value;
    }
}

public abstract class Question<TAnswer> : Question
{
    [SetsRequiredMembers]
    protected Question()
    {
    }

    [SetsRequiredMembers]
    protected Question(
        Guid id,
        Guid quizId,
        string questionText,
        int displayOrder,
        int marks)
        : base(id, quizId, questionText, displayOrder, marks)
    {
    }

    public abstract Result<QuestionAnswer> Solve(TAnswer answer, Guid studentId, Guid quizAttemptId);
}
