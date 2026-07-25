using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

namespace QuizNova.Application.Features.Quizzes.Commands.AddQuestion;

public sealed class AddQuestionCommandHandler(
    IMongoDbContext mongoContext,
    ILogger<AddQuestionCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<AddQuestionCommand, Result<QuestionDto>>
{
    public async Task<Result<QuestionDto>> Handle(AddQuestionCommand request, CancellationToken ct)
    {
        logger.LogInformation("Adding question to quiz: {QuizId}", request.QuizId);

        var quiz = await mongoContext.Quizzes
            .Find(q => q.Id == request.QuizId)
            .FirstOrDefaultAsync(ct);

        if (quiz is null)
        {
            logger.LogWarning("Quiz {QuizId} not found", request.QuizId);
            return ApplicationErrors.QuizNotFound(request.QuizId);
        }

        var questionCommand = request.Question;

        var displayOrder = quiz.Questions.Any()
            ? quiz.Questions.Max(q => q.DisplayOrder) + 1
            : 0;

        var createQuestionResult = questionCommand switch
        {
            CreateTfCommand tf => CreateTf(tf, displayOrder, request.QuizId),
            CreateMcqCommand mcq => CreateMcq(mcq, displayOrder, request.QuizId),
            CreateEssayCommand essay => CreateEssay(essay, displayOrder, request.QuizId),
            _ => Error.Unexpected(
                "Quiz.Question.Unsupported",
                $"Unsupported question type '{questionCommand.GetType().Name}'."),
        };

        if (createQuestionResult.IsError)
        {
            logger.LogWarning(
                "Failed to create question: {Error}",
                createQuestionResult.TopError.Description);
            return createQuestionResult.TopError;
        }

        var addResult = quiz.AddQuestion(createQuestionResult.Value);

        if (addResult.IsError)
        {
            logger.LogWarning(
                "Failed to add question to quiz {QuizId}: {Error}",
                request.QuizId,
                addResult.TopError.Description);
            return addResult.TopError;
        }

        await mongoContext.Quizzes.ReplaceOneAsync(q => q.Id == quiz.Id, quiz, cancellationToken: ct);
        await cacheInvalidator.InvalidateAsync([CacheTags.Quizzes], ct);

        logger.LogInformation(
            "Successfully added question {QuestionId} to quiz {QuizId}",
            createQuestionResult.Value.Id,
            request.QuizId);

        return createQuestionResult.Value.ToQuestionDto();
    }

    private static Result<Question> CreateTf(CreateTfCommand command, int displayOrder, Guid quizId)
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

    private static Result<Question> CreateMcq(CreateMcqCommand command, int displayOrder, Guid quizId)
    {
        var questionId = Guid.NewGuid();

        if (command.Choices.All(choice => choice.Id != command.CorrectChoiceId))
        {
            return ApplicationErrors.QuizCorrectChoiceNotFound(questionId, command.CorrectChoiceId);
        }

        if (command.Choices.GroupBy(choice => choice.Id).Any(group => group.Count() > 1))
        {
            return ApplicationErrors.QuizChoiceIdsMustBeUnique(questionId);
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

    private static Result<Question> CreateEssay(CreateEssayCommand command, int displayOrder, Guid quizId)
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
