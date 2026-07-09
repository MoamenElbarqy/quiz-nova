using MediatR;

using Microsoft.AspNetCore.Authorization;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.Commands.GradeQuestion;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Queries.GetPendingManualAnswers;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Endpoints;

public static class GradingEndpoints
{
    public static IEndpointRouteBuilder MapGradingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("quiz-attempts")
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Instructor) })
            .RequireRateLimiting("Global")
            .WithTags("grading")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("manually-graded-answers", async (ISender sender, [AsParameters] GetPendingManualAnswersQuery query) =>
        {
            var result = await sender.Send(query);
            return result.ToOk();
        })
        .WithName("GetPendingManualAnswers")
        .WithSummary("Retrieves all quiz attempts with pending manually-graded answers.")
        .WithDescription("Returns a paginated list of quiz attempts that have at least one essay answer not yet graded by the instructor.")
        .Produces<PaginatedList<PendingManualAnswersDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("manually-graded-answers/{answerId:guid}", async (ISender sender, Guid answerId, GradeQuestionRequest request) =>
        {
            var result = await sender.Send(new GradeQuestionCommand(
                answerId,
                request.Score,
                request.Feedback));
            return result.ToNoContent();
        })
        .WithName("GradeQuestion")
        .WithSummary("Grades a manually graded question answer.")
        .WithDescription("Sets the score and optional feedback for an essay answer submitted by a student.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }
}
