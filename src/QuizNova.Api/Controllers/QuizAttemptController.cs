using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuizAttempt;
using QuizNova.Application.Features.QuizAttempts.Queries.GetAllQuizzesAttempts;
using QuizNova.Application.Features.QuizAttempts.Queries.GetQuizAttemptById;
using QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttempts;
using QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttemptsCount;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("students/{studentId:guid}/quiz-attempts")]
[Authorize]
public sealed class QuizAttemptController(ISender sender) : ApiController
{
    [EndpointSummary("Retrieves a quiz attempt by id.")]
    [EndpointDescription("Fetches a single quiz attempt using the provided attempt identifier.")]
    [EndpointName("GetQuizAttemptById")]
    [HttpGet("{id:guid}")]
    [OutputCache(Tags = ["quiz-attempts"])]
    public async Task<IActionResult> GetQuizAttemptById([FromRoute] Guid id)
    {
        var result = await sender.Send(new GetQuizAttemptByIdQuery(id));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Submits a student's quiz attempt.")]
    [EndpointDescription("Creates and grades a submitted quiz attempt for the specified student.")]
    [EndpointName("SubmitQuizAttempt")]
    [HttpPost]
    public async Task<IActionResult> SubmitQuizAttempt(
        [FromRoute] Guid studentId,
        [FromBody] SubmitQuizAttemptRequest request)
    {
        var result = await sender.Send(new SubmitQuizAttemptCommand(
            request.Id,
            studentId,
            request.QuizId,
            request.StartedAt,
            request.SubmittedAt,
            request.QuestionAnswers
                .Select<SubmitQuestionAnswerRequest, SubmitQuestionAnswerCommand>(answer =>
                {
                    return answer switch
                    {
                        SubmitMcqAnswerRequest mcqAnswer => new SubmitMcqAnswerCommand(
                            mcqAnswer.Id,
                            mcqAnswer.QuestionId,
                            mcqAnswer.SelectedChoiceId),
                        SubmitTfAnswerRequest tfAnswer => new SubmitTfAnswerCommand(
                            tfAnswer.Id,
                            tfAnswer.QuestionId,
                            tfAnswer.StudentChoice),
                        _ => throw new InvalidOperationException("Unknown answer type"),
                    };
                })
                .ToList()));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves a student's quiz attempts.")]
    [EndpointDescription("Returns all quiz attempts associated with the specified student.")]
    [EndpointName("GetStudentQuizAttempts")]
    [OutputCache(Tags = ["quiz-attempts"])]
    [HttpGet]
    public async Task<IActionResult> GetStudentQuizAttempts([FromRoute] Guid studentId)
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
    [HttpGet("count")]
    public async Task<IActionResult> GetStudentQuizAttemptsCount([FromRoute] Guid studentId)
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
    [HttpGet("/quiz-attempts")]
    public async Task<IActionResult> GetAllQuizzesAttempts([FromQuery] GetAllQuizzesAttemptsQuery query)
    {
        var result = await sender.Send(query);

        return result.Match(
            Ok,
            Problem);
    }
}
