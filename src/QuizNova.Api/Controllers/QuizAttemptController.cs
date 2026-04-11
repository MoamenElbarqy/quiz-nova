using MediatR;

using Microsoft.AspNetCore.Mvc;

using QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttempts;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("students/{studentId:guid}/quiz-attempts")]
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
}
