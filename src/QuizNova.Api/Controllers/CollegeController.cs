using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Application.Features.Colleges.Queries.GetCollegeSummary;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("colleges")]
[Authorize(Roles = nameof(UserRole.Admin))]
public sealed class CollegeController(ISender sender) : ApiController
{
    [HttpGet]
    [EndpointSummary("Retrieves college summary metrics.")]
    [EndpointDescription("Returns aggregate college information intended for administrative dashboards.")]
    [EndpointName("GetCollegeSummary")]
    [OutputCache(Tags = ["colleges"])]
    public async Task<IActionResult> GetSummary()
    {
        var result = await sender.Send(new GetCollegeSummaryQuery());

        return result.Match(
            Ok,
            Problem);
    }
}
