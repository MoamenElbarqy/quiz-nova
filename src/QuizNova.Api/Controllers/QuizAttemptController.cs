using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttempts;
using QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttemptsCount;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("students/{studentId:guid}/quiz-attempts")]
[Authorize]
public sealed class QuizAttemptController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetStudentQuizAttempts([FromRoute] Guid studentId)
    {
        var result = await sender.Send(new GetStudentQuizAttemptsQuery(studentId));

        return result.Match(
            Ok,
            Problem);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetStudentQuizAttemptsCount([FromRoute] Guid studentId)
    {
        var result = await sender.Send(new GetStudentQuizAttemptsCountQuery(studentId));

        return result.Match(
            Ok,
            Problem);
    }
}
