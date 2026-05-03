using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;
using QuizNova.Application.Features.Quizzes.Queries.GetAllQuizzes;
using QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzesCount;
using QuizNova.Application.Features.Quizzes.Queries.GetQuizById;

namespace QuizNova.Api.Controllers;

[ApiController]
[Authorize]
[Route("quizzes")]
public sealed class QuizController(ISender sender) : ApiController
{
    [EndpointSummary("Retrieves quizzes.")]
    [EndpointDescription("Returns a paginated and filterable list of quizzes.")]
    [EndpointName("GetAllQuizzes")]
    [OutputCache(Tags = ["quizzes"])]
    [HttpGet]
    public async Task<IActionResult> GetAllQuizzes([FromQuery] GetAllQuizzesQuery query)
    {
        var result = await sender.Send(query);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves instructor quiz count.")]
    [EndpointDescription("Returns the number of quizzes created by the specified instructor.")]
    [EndpointName("GetInstructorQuizzesCount")]
    [OutputCache(Tags = ["quizzes"])]
    [HttpGet("count")]
    public async Task<IActionResult> GetInstructorQuizzesCount([FromQuery] Guid instructorId)
    {
        var result = await sender.Send(new GetInstructorQuizzesCountQuery(instructorId));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves a quiz by id.")]
    [EndpointDescription("Fetches a single quiz using the provided quiz identifier.")]
    [EndpointName("GetQuizById")]
    [OutputCache(Tags = ["quizzes"])]
    [HttpGet("{quizId:guid}")]
    public async Task<IActionResult> GetQuizById([FromRoute] Guid quizId)
    {
        var result = await sender.Send(new GetQuizByIdQuery(quizId));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Creates a new quiz.")]
    [EndpointDescription("Creates a quiz and its question set from the submitted request payload.")]
    [EndpointName("CreateQuiz")]
    [HttpPost]
    public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequest request)
    {
        var createQuizResult = await sender.Send(new CreateQuizCommand(
            request.Id,
            request.Title,
            request.CourseId,
            request.InstructorId,
            request.StartsAtUtc,
            request.EndsAtUtc,
            request.Questions
                .Select<CreateQuizQuestionRequest, CreateQuestionCommand>(q =>
                {
                    return q switch
                    {
                        CreateMcqRequest mcq => new CreateMcqCommand(
                            mcq.Id,
                            mcq.QuizId,
                            mcq.QuestionText,
                            mcq.Marks,
                            mcq.NumberOfChoices,
                            mcq.CorrectChoiceId,
                            mcq.Choices.Select(c => new CreateChoiceCommand(
                                    c.Id,
                                    c.QuestionId,
                                    c.Text,
                                    c.DisplayOrder))
                                .ToList()),
                        CreateTfRequest tfq => new CreateTfCommand(
                            tfq.Id,
                            tfq.QuizId,
                            tfq.QuestionText,
                            tfq.Marks,
                            tfq.CorrectChoice),
                        _ => throw new InvalidOperationException("Unknown question type")
                    };
                })
                .ToList()));

        return createQuizResult.Match(
            quizDto => Ok(quizDto),
            Problem);
    }
}
