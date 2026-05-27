using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.Commands.GradeQuestion;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Queries.GetPendingManualAnswers;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("quiz-attempts")]
[Authorize(Roles = nameof(UserRole.Instructor))]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class GradingController(ISender sender) : ApiController
{
    [EndpointSummary("Retrieves all quiz attempts with pending manually-graded answers.")]
    [EndpointDescription("Returns a paginated list of quiz attempts that have at least one essay answer not yet graded by the instructor.")]
    [EndpointName("GetPendingManualAnswers")]
    [HttpGet("manually-graded-answers")]
    [ProducesResponseType(typeof(PaginatedList<PendingManualAnswersDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPendingManualAnswers([FromQuery] GetPendingManualAnswersQuery query)
    {
        var result = await sender.Send(query);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Grades a manually graded question answer.")]
    [EndpointDescription("Sets the score and optional feedback for an essay answer submitted by a student.")]
    [EndpointName("GradeQuestion")]
    [HttpPut("manually-graded-answers/{answerId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GradeQuestion(
        [FromRoute] Guid answerId,
        [FromBody] GradeQuestionRequest request)
    {
        var result = await sender.Send(new GradeQuestionCommand(
            answerId,
            request.Score,
            request.Feedback));

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}
