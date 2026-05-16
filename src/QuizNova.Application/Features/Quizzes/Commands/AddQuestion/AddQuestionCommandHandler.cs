using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq.Choices;
using QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

namespace QuizNova.Application.Features.Quizzes.Commands.AddQuestion;

public sealed class AddQuestionCommandHandler(
    IAppDbContext dbContext,
    ILogger<AddQuestionCommandHandler> logger)
    : IRequestHandler<AddQuestionCommand, Result<QuestionDto>>
{
    public async Task<Result<QuestionDto>> Handle(AddQuestionCommand request, CancellationToken ct)
    {
        logger.LogInformation("Adding question to quiz: {QuizId}", request.QuizId);

        var quiz = await dbContext.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, ct);

        if (quiz is null)
        {
            logger.LogWarning("Quiz {QuizId} not found", request.QuizId);
            return ApplicationErrors.QuizNotFound(request.QuizId);
        }

        var questionCommand = request.Question;

        if (questionCommand.QuizId != request.QuizId)
        {
            return QuizErrors.QuestionBelongsToDifferentQuiz(questionCommand.Id);
        }

        var displayOrder = quiz.Questions.Any()
            ? quiz.Questions.Max(q => q.DisplayOrder) + 1
            : 0;

        var createQuestionResult = questionCommand switch
        {
            CreateTfCommand tf => CreateTf(tf, displayOrder),
            CreateMcqCommand mcq => CreateMcq(mcq, displayOrder),
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

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation(
            "Successfully added question {QuestionId} to quiz {QuizId}",
            createQuestionResult.Value.Id,
            request.QuizId);

        return createQuestionResult.Value.ToQuestionDto();
    }

    private static Result<Question> CreateTf(CreateTfCommand command, int displayOrder)
    {
        var result = Tf.Create(
            command.Id,
            command.QuizId,
            command.QuestionText,
            command.CorrectChoice,
            displayOrder,
            command.Marks);

        return result.IsError ? result.TopError : result.Value;
    }

    private static Result<Question> CreateMcq(CreateMcqCommand command, int displayOrder)
    {
        if (command.Choices.All(choice => choice.Id != command.CorrectChoiceId))
        {
            return ApplicationErrors.QuizCorrectChoiceNotFound(command.Id, command.CorrectChoiceId);
        }

        if (command.Choices.GroupBy(choice => choice.Id).Any(group => group.Count() > 1))
        {
            return ApplicationErrors.QuizChoiceIdsMustBeUnique(command.Id);
        }

        var choices = new List<Choice>(command.Choices.Count);

        foreach (var choiceCommand in command.Choices)
        {
            if (choiceCommand.QuestionId != command.Id)
            {
                return ApplicationErrors.QuizChoiceBelongsToDifferentQuestion(choiceCommand.Id, command.Id);
            }

            var createChoiceResult = Choice.Create(
                choiceCommand.Id,
                choiceCommand.QuestionId,
                choiceCommand.Text,
                choiceCommand.DisplayOrder);

            if (createChoiceResult.IsError)
            {
                return createChoiceResult.TopError;
            }

            choices.Add(createChoiceResult.Value);
        }

        var createQuestionResult = Mcq.Create(
            command.Id,
            command.QuizId,
            command.QuestionText,
            command.CorrectChoiceId,
            displayOrder,
            command.Marks,
            choices);

        return createQuestionResult.IsError ? createQuestionResult.TopError : createQuestionResult.Value;
    }
}
