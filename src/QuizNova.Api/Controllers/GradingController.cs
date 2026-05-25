using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.QuizAttempts.Commands.GradeQuestion;
using QuizNova.Application.Features.QuizAttempts.Queries.GetPendingManualAnswers;
using QuizNova.Application.Features.QuizAttempts.Queries.GetQuizAttemptById;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("quiz-attempts")]
[Authorize(Roles = nameof(UserRole.Instructor))]
public sealed class GradingController(ISender sender) : ApiController
{
    [EndpointSummary("Retrieves all quiz attempts with pending manually-graded answers.")]
    [EndpointDescription("Returns a list of quiz attempts that have at least one essay answer not yet graded by the instructor.")]
    [EndpointName("GetPendingManualAnswers")]
    [HttpGet("manually-graded-answers")]
    [OutputCache(Tags = ["quiz-attempts"])]
    public async Task<IActionResult> GetPendingManualAnswers()
    {
        var result = await sender.Send(new GetPendingManualAnswersQuery());

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves a quiz attempt by id for instructor grading.")]
    [EndpointDescription("Fetches the full quiz attempt with all questions and answers for the instructor grade review page.")]
    [EndpointName("GetQuizAttemptForGrading")]
    [HttpGet("{attemptId:guid}")]
    [OutputCache(Tags = ["quiz-attempts"])]
    public async Task<IActionResult> GetQuizAttemptForGrading([FromRoute] Guid attemptId)
    {
        var result = await sender.Send(new GetQuizAttemptByIdQuery(attemptId));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Grades a manually graded question answer.")]
    [EndpointDescription("Sets the score and optional feedback for an essay answer submitted by a student.")]
    [EndpointName("GradeQuestion")]
    [HttpPut("manually-graded-answers/{answerId:guid}")]
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
