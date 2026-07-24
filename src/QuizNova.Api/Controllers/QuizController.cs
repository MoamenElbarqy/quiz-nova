using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Mappers;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesPerformance;
using QuizNova.Application.Features.Quizzes.Commands.AddQuestion;
using QuizNova.Application.Features.Quizzes.Commands.DeleteQuestion;
using QuizNova.Application.Features.Quizzes.Commands.UpdateQuizCourseId;
using QuizNova.Application.Features.Quizzes.Commands.UpdateQuizMetadata;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Queries.GetAllQuizzes;
using QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzes;
using QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzesCount;
using QuizNova.Application.Features.Quizzes.Queries.GetQuizById;
using QuizNova.Application.Features.Quizzes.Queries.GetStudentQuizzes;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Controllers;

[ApiController]
[Authorize]
[Route("quizzes")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class QuizController(ISender sender) : ApiController
{
    [EndpointSummary("Retrieves quizzes.")]
    [EndpointDescription("Returns a paginated and filterable list of quizzes.")]
    [EndpointName("GetAllQuizzes")]
    [OutputCache(Tags = ["quizzes"])]
    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(PaginatedList<QuizDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedList<QuizDto>>> GetAllQuizzes([FromQuery] GetAllQuizzesQuery query,
        CancellationToken ct)
    {
        var result = await sender.Send(query, ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves instructor quiz count.")]
    [EndpointDescription("Returns the number of quizzes created by the specified instructor.")]
    [EndpointName("GetInstructorQuizzesCount")]
    [OutputCache(Tags = ["quizzes"])]
    [HttpGet("count")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Instructor)}")]
    [ProducesResponseType(typeof(QuizzesCountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<QuizzesCountDto>> GetInstructorQuizzesCount([FromQuery] Guid instructorId,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetInstructorQuizzesCountQuery(instructorId), ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves a quiz by id.")]
    [EndpointDescription("Fetches a single quiz using the provided quiz identifier.")]
    [EndpointName("GetQuizById")]
    [OutputCache(Tags = ["quizzes"])]
    [HttpGet("{quizId:guid}")]
    [ProducesResponseType(typeof(QuizDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuizDto>> GetQuizById([FromRoute] Guid quizId, CancellationToken ct)
    {
        var result = await sender.Send(new GetQuizByIdQuery(quizId), ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Creates a new quiz.")]
    [EndpointDescription("Creates a quiz and its question set from the submitted request payload.")]
    [EndpointName("CreateQuiz")]
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Instructor))]
    [ProducesResponseType(typeof(QuizDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<QuizDto>> CreateQuiz([FromBody] CreateQuizRequest request, CancellationToken ct)
    {
        var command = request.ToCommand();

        var createQuizResult = await sender.Send(command, ct);

        return createQuizResult.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Updates quiz metadata.")]
    [EndpointDescription("Updates the title, start time, and end time of an existing quiz.")]
    [EndpointName("UpdateQuizMetadata")]
    [HttpPut("{quizId:guid}/metadata")]
    [Authorize(Roles = nameof(UserRole.Instructor))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateQuizMetadata(
        [FromRoute] Guid quizId,
        [FromBody] UpdateQuizMetadataRequest request,
        CancellationToken ct)
    {
        var result = await sender.Send(new UpdateQuizMetadataCommand(
            quizId,
            request.Title,
            request.StartsAtUtc,
            request.EndsAtUtc), ct);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    [EndpointSummary("Adds a question to a quiz.")]
    [EndpointDescription("Adds a new MCQ or True/False question to the specified quiz.")]
    [EndpointName("AddQuestion")]
    [HttpPost("{quizId:guid}/questions")]
    [Authorize(Roles = nameof(UserRole.Instructor))]
    [ProducesResponseType(typeof(QuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuestionDto>> AddQuestion(
        [FromRoute] Guid quizId,
        [FromBody] CreateQuizQuestionRequest request,
        CancellationToken ct)
    {
        var result = await sender.Send(new AddQuestionCommand(quizId, request.ToCommand()));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Updates a question in a quiz.")]
    [EndpointDescription("Updates an existing MCQ or True/False question within the specified quiz.")]
    [EndpointName("UpdateQuestion")]
    [HttpPut("{quizId:guid}/questions/{questionId:guid}")]
    [Authorize(Roles = nameof(UserRole.Instructor))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateQuestion(
        [FromRoute] Guid quizId,
        [FromRoute] Guid questionId,
        [FromBody] UpdateQuestionRequest request,
        CancellationToken ct)
    {
        var result = await sender.Send(request.ToCommand(quizId, questionId));
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
            UpdateEssayRequest essay => new UpdateEssayCommand(
                quizId,
                questionId,
                essay.QuestionText,
                essay.DisplayOrder,
                essay.Marks,
                essay.AnswerReference),
            _ => throw new InvalidOperationException("Unknown question type")
        };

        var result = await sender.Send(command, ct);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    [EndpointSummary("Updates the course of a quiz.")]
    [EndpointDescription(
        "Changes the course associated with a quiz. This is a destructive operation that clears all existing questions.")]
    [EndpointName("UpdateQuizCourseId")]
    [HttpPut("{quizId:guid}/course")]
    [Authorize(Roles = nameof(UserRole.Instructor))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateQuizCourseId(
        [FromRoute] Guid quizId,
        [FromBody] UpdateQuizCourseIdRequest request,
        CancellationToken ct)
    {
        var result = await sender.Send(new UpdateQuizCourseIdCommand(quizId, request.CourseId), ct);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    [EndpointSummary("Deletes a question from a quiz.")]
    [EndpointDescription("Removes a question from the specified quiz. The quiz must have more than 5 questions.")]
    [EndpointName("DeleteQuestion")]
    [HttpDelete("{quizId:guid}/questions/{questionId:guid}")]
    [Authorize(Roles = nameof(UserRole.Instructor))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteQuestion(
        [FromRoute] Guid quizId,
        [FromRoute] Guid questionId,
        CancellationToken ct)
    {
        var result = await sender.Send(new DeleteQuestionCommand(quizId, questionId), ct);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    [EndpointSummary("Retrieves quizzes assigned to a student.")]
    [EndpointDescription("Returns quizzes associatined with the specified student identifier.")]
    [EndpointName("GetStudentQuizzes")]
    [OutputCache(Tags = ["students", "quizzes"], VaryByQueryKeys = ["t"])]
    [HttpGet("/students/{id:guid}/quizzes")]
    [Authorize(Roles = nameof(UserRole.Student))]
    [ProducesResponseType(typeof(StudentQuizzesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentQuizzesDto>> GetStudentQuizzes([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetStudentQuizzesQuery(id), ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves quizzes created by an instructor.")]
    [EndpointDescription("Returns quizzes associated with the specified instructor identifier.")]
    [EndpointName("GetInstructorQuizzes")]
    [OutputCache(Tags = ["instructors", "quizzes"])]
    [HttpGet("/instructors/{id:guid}/quizzes")]
    [Authorize(Roles = nameof(UserRole.Instructor))]
    [ProducesResponseType(typeof(List<QuizDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<QuizDto>>> GetInstructorQuizzes([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetInstructorQuizzesQuery(id), ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves instructor courses performance.")]
    [EndpointDescription("Returns performance metrics for all courses of a specific instructor.")]
    [EndpointName("GetInstructorCoursesPerformance")]
    [OutputCache(Tags = ["courses", "quizzes", "instructors", "performance"])]
    [HttpGet("/instructors/{instructorId:guid}/courses/performance")]
    [Authorize(Roles = nameof(UserRole.Instructor))]
    [ProducesResponseType(typeof(List<CoursePerformanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<CoursePerformanceDto>>> GetInstructorCoursesPerformance(
        [FromRoute] Guid instructorId,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetInstructorCoursesPerformanceQuery(instructorId), ct);

        return result.Match(
            Ok,
            Problem);
    }
}
