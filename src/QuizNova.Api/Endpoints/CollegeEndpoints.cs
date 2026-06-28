using MediatR;

using Microsoft.AspNetCore.Authorization;

using QuizNova.Application.Features.Colleges.DTOs;
using QuizNova.Application.Features.Colleges.Queries.GetCollegeSummary;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Endpoints;

public static class CollegeEndpoints
{
    public static IEndpointRouteBuilder MapCollegeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("colleges")
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Admin) })
            .RequireRateLimiting("Global")
            .WithTags("colleges")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet(string.Empty, async (ISender sender) =>
        {
            var result = await sender.Send(new GetCollegeSummaryQuery());
            return result.ToOk();
        })
        .WithName("GetCollegeSummary")
        .WithSummary("Retrieves college summary metrics.")
        .WithDescription("Returns aggregate college information intended for administrative dashboards.")
        .CacheOutput(policy => policy.Tag("colleges"))
        .Produces<CollegeSummaryDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        return app;
    }
}
