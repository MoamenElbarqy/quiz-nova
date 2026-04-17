using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;
using QuizNova.Application.Features.Quizzes.Queries.GetAllQuizzes;
using QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzesCount;
using QuizNova.Application.Features.Quizzes.Queries.GetQuizById;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("quizzes")]
[Authorize]
public sealed class QuizController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllQuizzes()
    {
        var result = await sender.Send(new GetAllQuizzesQuery());

        return result.Match(
            Ok,
            Problem);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetInstructorQuizzesCount([FromQuery] Guid instructorId)
    {
        var result = await sender.Send(new GetInstructorQuizzesCountQuery(instructorId));

        return result.Match(
            Ok,
            Problem);
    }

    [HttpGet("{quizId:guid}")]
    public async Task<IActionResult> GetQuizById([FromRoute] Guid quizId)
    {
        var result = await sender.Send(new GetQuizByIdQuery(quizId));

        return result.Match(
            Ok,
            Problem);
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequest request)
    {
        var createQuizResult = await sender.Send(new CreateQuizCommand(request.Id, request.Title, request.CourseId, request.InstructorId, request.StartsAtUtc, request.EndsAtUtc, request.Questions.Select<CreateQuizQuestionRequest, CreateQuestionCommand>(q =>
        {
            return q switch
            {
                CreateMultipleChoiceQuestionRequest mcq => new CreateMultipleChoiceQuestionCommand(mcq.Id, mcq.QuizId, mcq.QuestionText, mcq.Marks, mcq.NumberOfChoices, mcq.CorrectChoiceId, mcq.Choices.Select(c => new CreateChoiceCommand(c.Id, c.QuestionId, c.Text, c.DisplayOrder)).ToList()),
                CreateTrueFalseQuestionRequest tfq => new CreateTrueFalseQuestionCommand(tfq.Id, tfq.QuizId, tfq.QuestionText, tfq.Marks, tfq.CorrectChoice),
                CreateEssayQuestionRequest eq => new CreateEssayQuestionCommand(eq.Id, eq.QuizId, eq.QuestionText, eq.Marks),
                _ => throw new InvalidOperationException("Unknown question type")
            };
        }).ToList()));

        return createQuizResult.Match(
            quizDto => Ok(quizDto),
            Problem);
    }
}
