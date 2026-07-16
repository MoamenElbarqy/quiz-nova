using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.QuizAttempts;
using QuizNova.Domain.Entities.QuizAttempts.Answers.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.TrueFalse;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;

namespace QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuestionAnswer;

public sealed class SubmitQuestionAnswerCommandHandler(
    IAppDbContext dbContext,
    IUser user,
    ILogger<SubmitQuestionAnswerCommandHandler> logger)
    : IRequestHandler<SubmitQuestionAnswerCommand, Result<Submitted>>
{
    public async Task<Result<Submitted>> Handle(SubmitQuestionAnswerCommand request, CancellationToken ct)
    {
        var studentId = Guid.Parse(user.Id!);

        logger.LogInformation(
            "Submitting answer for question {QuestionId} on attempt {AttemptId}",
            request.Answer.QuestionId,
            request.AttemptId);

        var attempt = await dbContext.QuizAttempts
            .Include(a => a.StudentAnswers.Where(sa => sa.QuestionId == request.Answer.QuestionId))
            .Include(a => a.Quiz!)
            .ThenInclude(q => q.Questions.Where(q => q.Id == request.Answer.QuestionId))
            .ThenInclude((Question q) => (q as Mcq)!.Choices)
            .FirstOrDefaultAsync(a => a.Id == request.AttemptId, ct);

        if (attempt is null)
        {
            logger.LogWarning("Submit answer failed: Attempt {AttemptId} not found", request.AttemptId);
            return ApplicationErrors.QuizAttemptNotFound(request.AttemptId);
        }

        if (attempt.StudentId != studentId)
        {
            return Error.Forbidden("Forbidden", "You do not own this attempt.");
        }

        var question = attempt.Quiz?.Questions.FirstOrDefault(q => q.Id == request.Answer.QuestionId);

        if (question is null)
        {
            logger.LogWarning(
                "Submit answer failed: Question {QuestionId} not found in quiz {QuizId}",
                request.Answer.QuestionId,
                attempt.QuizId);

            return QuizAttemptErrors.QuestionNotFoundInQuiz(
                request.Answer.QuestionId,
                attempt.QuizId);
        }

        Result<QuestionAnswer> createAnswerResult = (question, request.Answer) switch
        {
            (Mcq mcqQuestion, SubmitMcqAnswerCommand mcqAnswer) =>
                mcqQuestion.Solve(mcqAnswer.SelectedChoiceId, studentId, attempt.Id),
            (Tf tfQuestion, SubmitTfAnswerCommand tfAnswer) =>
                tfQuestion.Solve(tfAnswer.StudentChoice, studentId, attempt.Id),
            (Essay essayQuestion, SubmitEssayAnswerCommand essayAnswer) =>
                essayQuestion.Solve(essayAnswer.StudentResponse, studentId, attempt.Id),
            (Mcq, _) => Error.Unexpected(
                "QuizAttempt.Answer.AnswerTypeMismatch",
                $"Question {request.Answer.QuestionId} is an MCQ question but the submitted answer is not an MCQ answer."),
            (Tf, _) => Error.Unexpected(
                "QuizAttempt.Answer.AnswerTypeMismatch",
                $"Question {request.Answer.QuestionId} is a True/False question but the submitted answer is not a True/False answer."),
            (Essay, _) => Error.Unexpected(
                "QuizAttempt.Answer.AnswerTypeMismatch",
                $"Question {request.Answer.QuestionId} is an Essay question but the submitted answer is not an Essay answer."),
            _ => Error.Unexpected(
                "QuizAttempt.Answer.Unsupported",
                $"Question {request.Answer.QuestionId} has an unsupported question type."),
        };

        if (createAnswerResult.IsError)
        {
            logger.LogWarning(
                "Submit answer failed: Error creating answer. {Error}",
                createAnswerResult.TopError.Description);

            return createAnswerResult.TopError;
        }

        var submitResult = attempt.SubmitAnswer(createAnswerResult.Value);

        if (submitResult.IsError)
        {
            logger.LogWarning(
                "Submit answer failed: Domain error. {Error}",
                submitResult.TopError.Description);

            return submitResult.TopError;
        }

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation(
            "Successfully submitted answer for question {QuestionId} on attempt {AttemptId}",
            request.Answer.QuestionId,
            request.AttemptId);

        return Result.Submitted;
    }
}
