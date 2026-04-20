using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuizAttempt;
using QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttempts;
using QuizNova.Application.Features.QuizAttempts.Queries.GetStudentQuizAttemptsCount;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("students/{studentId:guid}/quiz-attempts")]
[Authorize]
public sealed class QuizAttemptController(ISender sender) : ApiController
{
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
                            mcqAnswer.QuestionId,
                            mcqAnswer.SelectedChoiceId),
                        SubmitTrueFalseQuestionAnswerRequest trueFalseAnswer => new SubmitTrueFalseQuestionAnswerCommand(
                            trueFalseAnswer.QuestionId,
                            trueFalseAnswer.StudentChoice),
                        _ => throw new InvalidOperationException("Unknown answer type"),
                    };
                })
                .ToList()));

        return result.Match(
            Ok,
            Problem);
    }

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
