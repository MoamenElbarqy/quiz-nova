using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Quizzes.Commands.AddQuestion;
using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;
using QuizNova.Application.Features.Quizzes.Commands.DeleteQuestion;
using QuizNova.Application.Features.Quizzes.Commands.UpdateQuestion;
using QuizNova.Application.Features.Quizzes.Commands.UpdateQuizCourseId;
using QuizNova.Application.Features.Quizzes.Commands.UpdateQuizMetadata;
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

    [EndpointSummary("Updates quiz metadata.")]
    [EndpointDescription("Updates the title, start time, and end time of an existing quiz.")]
    [EndpointName("UpdateQuizMetadata")]
    [HttpPut("{quizId:guid}/metadata")]
    public async Task<IActionResult> UpdateQuizMetadata(
        [FromRoute] Guid quizId,
        [FromBody] UpdateQuizMetadataRequest request)
    {
        var result = await sender.Send(new UpdateQuizMetadataCommand(
            quizId,
            request.Title,
            request.StartsAtUtc,
            request.EndsAtUtc));

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    [EndpointSummary("Adds a question to a quiz.")]
    [EndpointDescription("Adds a new MCQ or True/False question to the specified quiz.")]
    [EndpointName("AddQuestion")]
    [HttpPost("{quizId:guid}/questions")]
    public async Task<IActionResult> AddQuestion(
        [FromRoute] Guid quizId,
        [FromBody] CreateQuizQuestionRequest request)
    {
        CreateQuestionCommand questionCommand = request switch
        {
            CreateMcqRequest mcq => new CreateMcqCommand(
                mcq.Id,
                quizId,
                mcq.QuestionText,
                mcq.Marks,
                mcq.CorrectChoiceId,
                mcq.Choices.Select(c => new CreateChoiceCommand(
                        c.Id,
                        c.QuestionId,
                        c.Text,
                        c.DisplayOrder))
                    .ToList()),
            CreateTfRequest tfq => new CreateTfCommand(
                tfq.Id,
                quizId,
                tfq.QuestionText,
                tfq.Marks,
                tfq.CorrectChoice),
            _ => throw new InvalidOperationException("Unknown question type")
        };

        var result = await sender.Send(new AddQuestionCommand(quizId, questionCommand));

        return result.Match(
            questionDto => CreatedAtRoute("GetQuizById", new { quizId }, questionDto),
            Problem);
    }

    [EndpointSummary("Updates a question in a quiz.")]
    [EndpointDescription("Updates an existing MCQ or True/False question within the specified quiz.")]
    [EndpointName("UpdateQuestion")]
    [HttpPut("{quizId:guid}/questions/{questionId:guid}")]
    public async Task<IActionResult> UpdateQuestion(
        [FromRoute] Guid quizId,
        [FromRoute] Guid questionId,
        [FromBody] UpdateQuestionRequest request)
    {
        UpdateQuestionCommand command = request switch
        {
            UpdateMcqRequest mcq => new UpdateMcqCommand(
                quizId,
                questionId,
                mcq.QuestionText,
                mcq.DisplayOrder,
                mcq.Marks,
                mcq.CorrectChoiceId,
                mcq.Choices.Select(c => new CreateChoiceCommand(
                        c.Id,
                        c.QuestionId,
                        c.Text,
                        c.DisplayOrder))
                    .ToList()),
            UpdateTfRequest tf => new UpdateTfCommand(
                quizId,
                questionId,
                tf.QuestionText,
                tf.DisplayOrder,
                tf.Marks,
                tf.CorrectChoice),
            _ => throw new InvalidOperationException("Unknown question type")
        };

        var result = await sender.Send(command);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    [EndpointSummary("Updates the course of a quiz.")]
    [EndpointDescription(
        "Changes the course associated with a quiz. This is a destructive operation that clears all existing questions.")]
    [EndpointName("UpdateQuizCourseId")]
    [HttpPut("{quizId:guid}/course")]
    public async Task<IActionResult> UpdateQuizCourseId(
        [FromRoute] Guid quizId,
        [FromBody] UpdateQuizCourseIdRequest request)
    {
        var result = await sender.Send(new UpdateQuizCourseIdCommand(quizId, request.CourseId));

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    [EndpointSummary("Deletes a question from a quiz.")]
    [EndpointDescription("Removes a question from the specified quiz. The quiz must have more than 5 questions.")]
    [EndpointName("DeleteQuestion")]
    [HttpDelete("{quizId:guid}/questions/{questionId:guid}")]
    public async Task<IActionResult> DeleteQuestion(
        [FromRoute] Guid quizId,
        [FromRoute] Guid questionId)
    {
        var result = await sender.Send(new DeleteQuestionCommand(quizId, questionId));

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}
