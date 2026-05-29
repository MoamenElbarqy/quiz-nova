using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Mappers;
using QuizNova.Application.Common.Models;
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
    [OutputCache(Tags = ["quiz-attempts"])]
    [ProducesResponseType(typeof(QuizAttemptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuizAttemptDto>> GetQuizAttemptById(
        [FromRoute] Guid studentId,
        [FromRoute] Guid id)
    {
        var result = await sender.Send(new GetQuizAttemptByIdQuery(id));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves a quiz attempt by id for grading.")]
    [EndpointDescription("Fetches a single quiz attempt using the provided attempt identifier.")]
    [EndpointName("GetQuizAttemptByIdForGrading")]
    [HttpGet("quiz-attempts/{id:guid}")]
    [OutputCache(Tags = ["quiz-attempts"])]
    [Authorize(Roles = nameof(UserRole.Instructor))]
    [ProducesResponseType(typeof(QuizAttemptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuizAttemptDto>> GetQuizAttemptByIdForGrading([FromRoute] Guid id)
    {
        var result = await sender.Send(new GetQuizAttemptByIdQuery(id));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Submits a student's quiz attempt.")]
    [EndpointDescription("Creates and grades a submitted quiz attempt for the specified student.")]
    [EndpointName("SubmitQuizAttempt")]
    [HttpPost("students/{studentId:guid}/quiz-attempts")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(QuizAttemptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<QuizAttemptDto>> SubmitQuizAttempt(
        [FromRoute] Guid studentId,
        [FromBody] SubmitQuizAttemptRequest request)
    {
        var command = request.ToCommand(studentId);

        var result = await sender.Send(command);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves a student's quiz attempts.")]
    [EndpointDescription("Returns all quiz attempts associated with the specified student.")]
    [EndpointName("GetStudentQuizAttempts")]
    [OutputCache(Tags = ["quiz-attempts"])]
    [HttpGet("students/{studentId:guid}/quiz-attempts")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(IReadOnlyList<QuizAttemptDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<QuizAttemptDto>>> GetStudentQuizAttempts([FromRoute] Guid studentId)
    {
        var result = await sender.Send(new GetStudentQuizAttemptsQuery(studentId));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves a student's quiz attempt count.")]
    [EndpointDescription("Returns the total number of quiz attempts for the specified student.")]
    [EndpointName("GetStudentQuizAttemptsCount")]
    [OutputCache(Tags = ["quiz-attempts"])]
    [HttpGet("students/{studentId:guid}/quiz-attempts/count")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Student)}")]
    [ProducesResponseType(typeof(QuizAttemptsCountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<QuizAttemptsCountDto>> GetStudentQuizAttemptsCount([FromRoute] Guid studentId)
    {
        var result = await sender.Send(new GetStudentQuizAttemptsCountQuery(studentId));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves all quiz attempts.")]
    [EndpointDescription("Returns a filtered list of quiz attempts across students.")]
    [EndpointName("GetAllQuizzesAttempts")]
    [OutputCache(Tags = ["quiz-attempts"])]
    [HttpGet("quiz-attempts")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(PaginatedList<QuizAttemptDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedList<QuizAttemptDto>>> GetAllQuizzesAttempts([FromQuery] GetAllQuizzesAttemptsQuery query)
    {
        var result = await sender.Send(query);

        return result.Match(
            Ok,
            Problem);
    }
}
