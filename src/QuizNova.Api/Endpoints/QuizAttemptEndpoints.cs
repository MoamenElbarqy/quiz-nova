using MediatR;

using Microsoft.AspNetCore.Authorization;

using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Queries.GetAllQuizzesAttempts;
using QuizNova.Application.Features.QuizAttempts.Queries.GetQuizAttemptById;
using QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttempts;
using QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttemptsCount;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Endpoints;

public static class QuizAttemptEndpoints
{
    public static IEndpointRouteBuilder MapQuizAttemptEndpoints(this IEndpointRouteBuilder app)
    {
        var studentsGroup = app.MapGroup("students")
            .RequireAuthorization()
            .RequireRateLimiting(RateLimiterPolicies.Global)
            .WithTags("quiz-attempts")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        var attemptsGroup = app.MapGroup("quiz-attempts")
            .RequireAuthorization()
            .RequireRateLimiting(RateLimiterPolicies.Global)
            .WithTags("quiz-attempts")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        studentsGroup.MapGet("{studentId:guid}/quiz-attempts/{id:guid}", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new GetQuizAttemptByIdQuery(id));
            return result.ToOk();
        })
        .WithName("GetQuizAttemptById")
        .WithSummary("Retrieves a quiz attempt by id.")
        .WithDescription("Fetches a single quiz attempt using the provided attempt identifier.")
        .CacheOutput(policy => policy.Tag(CacheTags.QuizAttempts))
        .Produces<QuizAttemptDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        attemptsGroup.MapGet("{id:guid}", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new GetQuizAttemptByIdQuery(id));
            return result.ToOk();
        })
        .WithName("GetQuizAttemptByIdForGrading")
        .WithSummary("Retrieves a quiz attempt by id for grading.")
        .WithDescription("Fetches a single quiz attempt using the provided attempt identifier.")
        .CacheOutput(policy => policy.Tag(CacheTags.QuizAttempts))
        .RequireAuthorization(new AuthorizeAttribute { Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Instructor)}" })
        .Produces<QuizAttemptDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        studentsGroup.MapGet("{studentId:guid}/quiz-attempts", async (ISender sender, Guid studentId) =>
        {
            var result = await sender.Send(new GetStudentQuizAttemptsQuery(studentId));
            return result.ToOk();
        })
        .WithName("GetStudentQuizAttempts")
        .WithSummary("Retrieves a student's quiz attempts.")
        .WithDescription("Returns all quiz attempts associated with the specified student.")
        .CacheOutput(policy => policy.Tag(CacheTags.QuizAttempts))
        .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Student) })
        .Produces<IReadOnlyList<QuizAttemptDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        studentsGroup.MapGet("{studentId:guid}/quiz-attempts/count", async (ISender sender, Guid studentId) =>
        {
            var result = await sender.Send(new GetStudentQuizAttemptsCountQuery(studentId));
            return result.ToOk();
        })
        .WithName("GetStudentQuizAttemptsCount")
        .WithSummary("Retrieves a student's quiz attempt count.")
        .WithDescription("Returns the total number of quiz attempts for the specified student.")
        .CacheOutput(policy => policy.Tag(CacheTags.QuizAttempts))
        .RequireAuthorization(new AuthorizeAttribute { Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Student)}" })
        .Produces<QuizAttemptsCountDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        attemptsGroup.MapGet(string.Empty, async (ISender sender, [AsParameters] GetAllQuizzesAttemptsQuery query) =>
        {
            var result = await sender.Send(query);
            return result.ToOk();
        })
        .WithName("GetAllQuizzesAttempts")
        .WithSummary("Retrieves all quiz attempts.")
        .WithDescription("Returns a filtered list of quiz attempts across students.")
        .CacheOutput(policy => policy.Tag(CacheTags.QuizAttempts))
        .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Admin) })
        .Produces<PaginatedList<QuizAttemptDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        return app;
    }
}
