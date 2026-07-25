using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Mappers;
using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.QuizAttempts.Commands.CompleteQuizAttempt;
using QuizNova.Application.Features.QuizAttempts.Commands.StartQuizAttempt;
using QuizNova.Application.Features.QuizAttempts.DTOs;
using QuizNova.Application.Features.QuizAttempts.Queries.GetAllQuizzesAttempts;
using QuizNova.Application.Features.QuizAttempts.Queries.GetQuizAttemptById;
using QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttempts;
using QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttemptsCount;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class QuizAttemptController(ISender sender) : ApiController
{
    [EndpointSummary("Retrieves a quiz attempt by id.")]
    [EndpointDescription("Fetches a single quiz attempt using the provided attempt identifier.")]
    [EndpointName("GetQuizAttemptById")]
    [HttpGet("students/{studentId:guid}/quiz-attempts/{id:guid}")]
    [OutputCache(Tags = [CacheTags.QuizAttempts])]
    [ProducesResponseType(typeof(QuizAttemptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuizAttemptDto>> GetQuizAttemptById(
        [FromRoute] Guid studentId,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetQuizAttemptByIdQuery(id), ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves a quiz attempt by id for grading or resume.")]
    [EndpointDescription("Fetches a single quiz attempt using the provided attempt identifier.")]
    [EndpointName("GetQuizAttemptByIdForGrading")]
    [HttpGet("quiz-attempts/{id:guid}")]
    [OutputCache(Tags = [CacheTags.QuizAttempts])]
    [Authorize(Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Instructor)}")]
    [ProducesResponseType(typeof(QuizAttemptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuizAttemptDto>> GetQuizAttemptByIdForGrading([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetQuizAttemptByIdQuery(id), ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Starts a new quiz attempt.")]
    [EndpointDescription("Creates a new quiz attempt in InProgress state for the authenticated student.")]
    [EndpointName("StartQuizAttempt")]
    [HttpPost("quizattempts")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [EnableRateLimiting(RateLimiterPolicies.SubmitQuiz)]
    [ProducesResponseType(typeof(QuizAttemptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<QuizAttemptDto>> StartQuizAttempt(
        [FromBody] StartQuizAttemptRequest request,
        CancellationToken ct)
    {
        var result = await sender.Send(new StartQuizAttemptCommand(request.QuizId), ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Submits an answer for a question in a quiz attempt.")]
    [EndpointDescription("Submits or updates a single question answer for an in-progress quiz attempt.")]
    [EndpointName("SubmitQuestionAnswer")]
    [HttpPost("quizattempts/{id:guid}/answers")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> SubmitQuestionAnswer(
        [FromRoute] Guid id,
        [FromBody] SubmitQuestionAnswerRequest request,
        CancellationToken ct)
    {
        var command = request.ToCommand(id);

        var result = await sender.Send(command, ct);

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [EndpointSummary("Completes a quiz attempt.")]
    [EndpointDescription("Marks an in-progress quiz attempt as completed.")]
    [EndpointName("CompleteQuizAttempt")]
    [HttpPut("quizattempts/{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(QuizAttemptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<QuizAttemptDto>> CompleteQuizAttempt(
        [FromRoute] Guid id,
        [FromBody] CompleteQuizAttemptRequest request,
        CancellationToken ct)
    {
        var result = await sender.Send(new CompleteQuizAttemptCommand(id, request.SubmittedAt), ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves a student's quiz attempts.")]
    [EndpointDescription("Returns all quiz attempts associated with the specified student.")]
    [EndpointName("GetStudentQuizAttempts")]
    [OutputCache(Tags = [CacheTags.QuizAttempts])]
    [HttpGet("students/{studentId:guid}/quiz-attempts")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(IReadOnlyList<QuizAttemptDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<QuizAttemptDto>>> GetStudentQuizAttempts([FromRoute] Guid studentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetStudentQuizAttemptsQuery(studentId), ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves a student's quiz attempt count.")]
    [EndpointDescription("Returns the total number of quiz attempts for the specified student.")]
    [EndpointName("GetStudentQuizAttemptsCount")]
    [OutputCache(Tags = [CacheTags.QuizAttempts])]
    [HttpGet("students/{studentId:guid}/quiz-attempts/count")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Student)}")]
    [ProducesResponseType(typeof(QuizAttemptsCountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<QuizAttemptsCountDto>> GetStudentQuizAttemptsCount([FromRoute] Guid studentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetStudentQuizAttemptsCountQuery(studentId), ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves all quiz attempts.")]
    [EndpointDescription("Returns a filtered list of quiz attempts across students.")]
    [EndpointName("GetAllQuizzesAttempts")]
    [OutputCache(Tags = [CacheTags.QuizAttempts])]
    [HttpGet("quiz-attempts")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(PaginatedList<QuizAttemptDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedList<QuizAttemptDto>>> GetAllQuizzesAttempts([FromQuery] GetAllQuizzesAttemptsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);

        return result.Match(
            Ok,
            Problem);
    }
}
