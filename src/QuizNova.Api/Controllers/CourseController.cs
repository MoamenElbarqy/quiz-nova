using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Mappers;
using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Courses.Commands.DeleteCourseById;
using QuizNova.Application.Features.Courses.Commands.UpdateCourseInstructor;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Application.Features.Courses.Queries.GetAllCourses;
using QuizNova.Application.Features.Courses.Queries.GetCourseById;
using QuizNova.Application.Features.Courses.Queries.GetInstructorCourses;
using QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesCount;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class CourseController(ISender sender) : ApiController
{
    [EndpointSummary("Retrieves courses.")]
    [EndpointDescription("Returns a paginated and filterable list of courses.")]
    [EndpointName("GetCourses")]
    [HttpGet("courses")]
    [OutputCache(Tags = [CacheTags.Courses])]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedList<CourseDto>>> GetCourses([FromQuery] GetAllCoursesQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result.Match(Ok, Problem);
    }

    [HttpGet("instructor/{instructorId:guid}/courses")]
    [OutputCache(Tags = [CacheTags.Courses])]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves instructor courses.")]
    [EndpointDescription("Returns all courses for a specific instructor.")]
    [EndpointName("GetInstructorCourses")]
    public async Task<ActionResult<List<CourseDto>>> GetInstructorCourses(Guid instructorId, CancellationToken ct)
    {
        var result = await sender.Send(new GetInstructorCoursesQuery(instructorId), ct);
        return result.Match(Ok, Problem);
    }

    [HttpGet("instructor/{instructorId:guid}/courses/count")]
    [OutputCache(Tags = [CacheTags.Courses])]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves instructor course counts.")]
    [EndpointDescription("Returns instructor course counts based on the instructor ID.")]
    [EndpointName("GetInstructorCoursesCount")]
    public async Task<ActionResult<CoursesCountDto>> GetInstructorCoursesCount(Guid instructorId, CancellationToken ct)
    {
        var result = await sender.Send(new GetInstructorCoursesCountQuery(instructorId), ct);
        return result.Match(Ok, Problem);
    }

    [HttpGet("courses/{id:guid}")]
    [OutputCache(Tags = [CacheTags.Courses])]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves a course by its unique identifier.")]
    [EndpointDescription("Fetches the details of a specific course using its ID.")]
    [EndpointName("GetCourseById")]
    public async Task<ActionResult<CourseDto>> GetCourseById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetCourseByIdQuery(id), ct);
        return result.Match(Ok, Problem);
    }

    [HttpPost("courses")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Creates a course.")]
    [EndpointDescription("Creates a course with an optional instructor assignment.")]
    [EndpointName("CreateCourse")]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseRequest request, CancellationToken ct)
    {
        var command = request.ToCommand();

        var result = await sender.Send(command, ct);
        return result.Match(Ok, Problem);
    }

    [HttpPatch("courses/{courseId:guid}/instructor")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Updates a course instructor.")]
    [EndpointDescription("Assigns or clears the instructor for a course.")]
    [EndpointName("UpdateCourseInstructor")]
    public async Task<ActionResult<CourseDto>> UpdateCourseInstructor(
        Guid courseId,
        [FromBody] UpdateCourseInstructorRequest request,
        CancellationToken ct)
    {
        var result = await sender.Send(new UpdateCourseInstructorCommand(courseId, request.InstructorId), ct);
        return result.Match(Ok, Problem);
    }

    [HttpDelete("courses/{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Deletes a course by its unique identifier.")]
    [EndpointDescription("Removes a course from the database using its ID.")]
    [EndpointName("DeleteCourseById")]
    public async Task<ActionResult> DeleteCourseById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteCourseByIdCommand(id), ct);
        return result.Match(_ => NoContent(), Problem);
    }
}
