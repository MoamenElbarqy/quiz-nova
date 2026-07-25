using MediatR;

using Microsoft.AspNetCore.Authorization;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Mappers;
using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Students.Commands.DeleteStudent;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Application.Features.Students.Queries.GetAllStudents;
using QuizNova.Application.Features.Students.Queries.GetStudentById;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Endpoints;

public static class StudentEndpoints
{
    public static IEndpointRouteBuilder MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("students")
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Admin) })
            .RequireRateLimiting(RateLimiterPolicies.Global)
            .WithTags("students")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet(string.Empty, async (ISender sender, [AsParameters] GetAllStudentsQuery query) =>
        {
            var result = await sender.Send(query);
            return result.ToOk();
        })
        .WithName("GetAllStudents")
        .WithSummary("Retrieves all students.")
        .WithDescription("Returns a paginated and filterable list of student users.")
        .CacheOutput(policy => policy.Tag(CacheTags.Students))
        .Produces<PaginatedList<StudentDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("{id:guid}", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new GetStudentByIdQuery(id));
            return result.ToOk();
        })
        .WithName("GetStudentById")
        .WithSummary("Retrieves a student by id.")
        .WithDescription("Fetches a single student using the provided student identifier.")
        .CacheOutput(policy => policy.Tag(CacheTags.Students))
        .Produces<StudentDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost(string.Empty, async (ISender sender, CreateStudentRequest request) =>
        {
            var command = request.ToCommand();
            var result = await sender.Send(command);
            return result.ToOk();
        })
        .WithName("CreateStudent")
        .WithSummary("Creates a new student.")
        .WithDescription("Creates a student account from the submitted request payload.")
        .Produces<StudentDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("{id:guid}", async (ISender sender, Guid id, UpdateStudentRequest request) =>
        {
            var command = request.ToCommand(id);
            var result = await sender.Send(command);
            return result.ToOk();
        })
        .WithName("UpdateStudent")
        .WithSummary("Updates an existing student.")
        .WithDescription("Updates profile and credential fields for the specified student.")
        .Produces<StudentDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("{id:guid}", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new DeleteStudentCommand(id));
            return result.ToNoContent();
        })
        .WithName("DeleteStudent")
        .WithSummary("Deletes a student.")
        .WithDescription("Removes the student account identified by the provided student identifier.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
