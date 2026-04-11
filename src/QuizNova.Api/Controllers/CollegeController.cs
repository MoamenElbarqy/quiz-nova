using MediatR;

using Microsoft.AspNetCore.Mvc;

using QuizNova.Application.Features.Colleges.Queries.GetCollegeSummary;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("colleges")]
public sealed class CollegeController(ISender sender) : ApiController
{
    public async Task<IActionResult> GetSummary()
    {
        var result = await sender.Send(new GetCollegeSummaryQuery());

        return result.Match(
            Ok,
            Problem);
    }
}
