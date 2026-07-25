using MediatR;

using Microsoft.AspNetCore.Authorization;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Mappers;
using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Application.Features.Admins.Queries.GetAdminById;
using QuizNova.Application.Features.Admins.Queries.GetAllAdmins;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("admins")
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Admin) })
            .RequireRateLimiting(RateLimiterPolicies.Global)
            .WithTags("admins")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet(string.Empty, async (ISender sender, [AsParameters] GetAllAdminsQuery query) =>
        {
            var result = await sender.Send(query);
            return result.ToOk();
        })
        .WithName("GetAllAdmins")
        .WithSummary("Retrieves all admins.")
        .WithDescription("Returns a paginated and filterable list of admin users.")
        .CacheOutput(policy => policy.Tag(CacheTags.Admins))
        .Produces<PaginatedList<AdminDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("{id:guid}", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new GetAdminByIdQuery(id));
            return result.ToOk();
        })
        .WithName("GetAdminById")
        .WithSummary("Retrieves an admin by id.")
        .WithDescription("Fetches a single admin using the provided admin identifier.")
        .CacheOutput(policy => policy.Tag(CacheTags.Admins))
        .Produces<AdminDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost(string.Empty, async (ISender sender, CreateAdminRequest request) =>
        {
            var command = request.ToCommand();
            var result = await sender.Send(command);
            return result.ToOk();
        })
        .WithName("CreateAdmin")
        .WithSummary("Creates a new admin.")
        .WithDescription("Creates an admin account from the submitted request payload.")
        .Produces<AdminDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }
}
