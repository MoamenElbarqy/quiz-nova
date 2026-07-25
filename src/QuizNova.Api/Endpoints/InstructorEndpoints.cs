using MediatR;

using Microsoft.AspNetCore.Authorization;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Mappers;
using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Instructors.Commands.DeleteInstructor;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Application.Features.Instructors.Queries.GetAllInstructors;
using QuizNova.Application.Features.Instructors.Queries.GetInstructorById;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Endpoints;

public static class InstructorEndpoints
{
    public static IEndpointRouteBuilder MapInstructorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("instructors")
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Admin) })
            .RequireRateLimiting(RateLimiterPolicies.Global)
            .WithTags("instructors")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet(string.Empty, async (ISender sender, [AsParameters] GetAllInstructorsQuery query) =>
        {
            var result = await sender.Send(query);
            return result.ToOk();
        })
        .WithName("GetAllInstructors")
        .WithSummary("Retrieves all instructors.")
        .WithDescription("Returns a paginated and filterable list of instructor users.")
        .CacheOutput(policy => policy.Tag(CacheTags.Instructors))
        .Produces<PaginatedList<InstructorDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("{id:guid}", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new GetInstructorByIdQuery(id));
            return result.ToOk();
        })
        .WithName("GetInstructorById")
        .WithSummary("Retrieves an instructor by id.")
        .WithDescription("Fetches a single instructor using the provided instructor identifier.")
        .CacheOutput(policy => policy.Tag(CacheTags.Instructors))
        .Produces<InstructorDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost(string.Empty, async (ISender sender, CreateInstructorRequest request) =>
        {
            var command = request.ToCommand();
            var result = await sender.Send(command);
            return result.ToOk();
        })
        .WithName("CreateInstructor")
        .WithSummary("Creates a new instructor.")
        .WithDescription("Creates an instructor account from the submitted request payload.")
        .Produces<InstructorDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("{id:guid}", async (ISender sender, Guid id, UpdateInstructorRequest request) =>
        {
            var command = request.ToCommand(id);
            var result = await sender.Send(command);
            return result.ToOk();
        })
        .WithName("UpdateInstructor")
        .WithSummary("Updates an existing instructor.")
        .WithDescription("Updates profile and credential fields for the specified instructor.")
        .Produces<InstructorDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("{id:guid}", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new DeleteInstructorCommand(id));
            return result.ToNoContent();
        })
        .WithName("DeleteInstructor")
        .WithSummary("Deletes an instructor.")
        .WithDescription("Removes the instructor account identified by the provided instructor identifier.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
