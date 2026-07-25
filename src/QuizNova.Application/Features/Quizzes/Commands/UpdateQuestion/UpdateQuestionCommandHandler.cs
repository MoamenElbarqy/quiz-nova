using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;

namespace QuizNova.Application.Features.Quizzes.Commands.UpdateQuestion;

public sealed class UpdateQuestionCommandHandler(
    IMongoDbContext mongoContext,
    ILogger<UpdateQuestionCommandHandler> logger,
    ICacheInvalidator cacheInvalidator)
    : IRequestHandler<UpdateMcqCommand, Result<Updated>>,
        IRequestHandler<UpdateTfCommand, Result<Updated>>,
        IRequestHandler<UpdateEssayCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(UpdateMcqCommand request, CancellationToken ct)
    {
        return await HandleCore(request, ct);
    }

    public async Task<Result<Updated>> Handle(UpdateTfCommand request, CancellationToken ct)
    {
        return await HandleCore(request, ct);
    }

    public async Task<Result<Updated>> Handle(UpdateEssayCommand request, CancellationToken ct)
    {
        return await HandleCore(request, ct);
    }

    private async Task<Result<Updated>> HandleCore(UpdateQuestionCommand request, CancellationToken ct)
    {
        logger.LogInformation(
            "Updating question {QuestionId} in quiz {QuizId}",
            request.QuestionId,
            request.QuizId);

        var quiz = await mongoContext.Quizzes
            .Find(q => q.Id == request.QuizId)
            .FirstOrDefaultAsync(ct);

        if (quiz is null)
        {
            logger.LogWarning("Quiz {QuizId} not found", request.QuizId);
            return ApplicationErrors.QuizNotFound(request.QuizId);
        }

        Guid? correctChoiceId = null;
        bool? tfCorrectChoice = null;
        List<Choice>? choices = null;
        string? answerReference = null;

        switch (request)
        {
            case UpdateMcqCommand mcqCmd:
                correctChoiceId = mcqCmd.CorrectChoiceId;

                if (mcqCmd.Choices.All(c => c.Id != mcqCmd.CorrectChoiceId))
                {
                    return ApplicationErrors.QuizCorrectChoiceNotFound(mcqCmd.QuestionId, mcqCmd.CorrectChoiceId);
                }

                choices = new List<Choice>(mcqCmd.Choices.Count);

                foreach (var choiceCmd in mcqCmd.Choices)
                {
                    var choiceResult = Choice.Create(
                        choiceCmd.Id,
                        mcqCmd.QuestionId,
                        choiceCmd.Text,
                        choiceCmd.DisplayOrder);

                    if (choiceResult.IsError)
                    {
                        return choiceResult.TopError;
                    }

                    choices.Add(choiceResult.Value);
                }

                break;

            case UpdateTfCommand tfCmd:
                tfCorrectChoice = tfCmd.CorrectChoice;
                break;

            case UpdateEssayCommand essayCmd:
                answerReference = essayCmd.AnswerReference;
                break;
        }

        var updateResult = quiz.UpdateQuestion(
            request.QuestionId,
            request.QuestionText,
            request.DisplayOrder,
            request.Marks,
            correctChoiceId,
            tfCorrectChoice,
            choices,
            answerReference);

        if (updateResult.IsError)
        {
            logger.LogWarning(
                "Failed to update question {QuestionId}: {Error}",
                request.QuestionId,
                updateResult.TopError.Description);
            return updateResult.TopError;
        }

        await mongoContext.Quizzes.ReplaceOneAsync(q => q.Id == quiz.Id, quiz, cancellationToken: ct);
        await cacheInvalidator.InvalidateAsync([CacheTags.Quizzes], ct);

        logger.LogInformation(
            "Successfully updated question {QuestionId} in quiz {QuizId}",
            request.QuestionId,
            request.QuizId);

        return Result.Updated;
    }
}
