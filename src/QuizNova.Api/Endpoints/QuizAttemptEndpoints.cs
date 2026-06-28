using MediatR;

using Microsoft.AspNetCore.Authorization;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Mappers;
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
            .RequireRateLimiting("Global")
            .WithTags("quiz-attempts")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        var attemptsGroup = app.MapGroup("quiz-attempts")
            .RequireAuthorization()
            .RequireRateLimiting("Global")
            .WithTags("quiz-attempts")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // GET students/{studentId:guid}/quiz-attempts/{id:guid}
        studentsGroup.MapGet("{studentId:guid}/quiz-attempts/{id:guid}", async (ISender sender, Guid studentId, Guid id) =>
        {
            var result = await sender.Send(new GetQuizAttemptByIdQuery(id));
            return result.ToOk();
        })
        .WithName("GetQuizAttemptById")
        .WithSummary("Retrieves a quiz attempt by id.")
        .WithDescription("Fetches a single quiz attempt using the provided attempt identifier.")
        .CacheOutput(policy => policy.Tag("quiz-attempts"))
        .Produces<QuizAttemptDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // GET quiz-attempts/{id:guid}
        attemptsGroup.MapGet("{id:guid}", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new GetQuizAttemptByIdQuery(id));
            return result.ToOk();
        })
        .WithName("GetQuizAttemptByIdForGrading")
        .WithSummary("Retrieves a quiz attempt by id for grading.")
        .WithDescription("Fetches a single quiz attempt using the provided attempt identifier.")
        .CacheOutput(policy => policy.Tag("quiz-attempts"))
        .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Instructor) })
        .Produces<QuizAttemptDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // POST students/{studentId:guid}/quiz-attempts
        studentsGroup.MapPost("{studentId:guid}/quiz-attempts", async (ISender sender, Guid studentId, SubmitQuizAttemptRequest request) =>
        {
            var command = request.ToCommand(studentId);
            var result = await sender.Send(command);
            return result.ToOk();
        })
        .WithName("SubmitQuizAttempt")
        .WithSummary("Submits a student's quiz attempt.")
        .WithDescription("Creates and grades a submitted quiz attempt for the specified student.")
        .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Student) })
        .RequireRateLimiting("SubmitQuiz")
        .Produces<QuizAttemptDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        // GET students/{studentId:guid}/quiz-attempts
        studentsGroup.MapGet("{studentId:guid}/quiz-attempts", async (ISender sender, Guid studentId) =>
        {
            var result = await sender.Send(new GetStudentQuizAttemptsQuery(studentId));
            return result.ToOk();
        })
        .WithName("GetStudentQuizAttempts")
        .WithSummary("Retrieves a student's quiz attempts.")
        .WithDescription("Returns all quiz attempts associated with the specified student.")
        .CacheOutput(policy => policy.Tag("quiz-attempts"))
        .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Student) })
        .Produces<IReadOnlyList<QuizAttemptDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        // GET students/{studentId:guid}/quiz-attempts/count
        studentsGroup.MapGet("{studentId:guid}/quiz-attempts/count", async (ISender sender, Guid studentId) =>
        {
            var result = await sender.Send(new GetStudentQuizAttemptsCountQuery(studentId));
            return result.ToOk();
        })
        .WithName("GetStudentQuizAttemptsCount")
        .WithSummary("Retrieves a student's quiz attempt count.")
        .WithDescription("Returns the total number of quiz attempts for the specified student.")
        .CacheOutput(policy => policy.Tag("quiz-attempts"))
        .RequireAuthorization(new AuthorizeAttribute { Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Student)}" })
        .Produces<QuizAttemptsCountDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        // GET quiz-attempts
        attemptsGroup.MapGet(string.Empty, async (ISender sender, [AsParameters] GetAllQuizzesAttemptsQuery query) =>
        {
            var result = await sender.Send(query);
            return result.ToOk();
        })
        .WithName("GetAllQuizzesAttempts")
        .WithSummary("Retrieves all quiz attempts.")
        .WithDescription("Returns a filtered list of quiz attempts across students.")
        .CacheOutput(policy => policy.Tag("quiz-attempts"))
        .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Admin) })
        .Produces<PaginatedList<QuizAttemptDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        return app;
    }
}
