using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Courses.Commands.DeleteCourseById;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Application.Features.Courses.Queries.GetAllCourses;
using QuizNova.Application.Features.Courses.Queries.GetCourseById;
using QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesCount;
using QuizNova.Application.Features.Courses.Queries.GetStudentCoursesCount;

namespace QuizNova.Api.Controllers;

[ApiController]
[Authorize]
public sealed class CourseController(ISender sender) : ApiController
{
    [EndpointSummary("Retrieves courses.")]
    [EndpointDescription("Returns a paginated and filterable list of courses.")]
    [EndpointName("GetCourses")]
    [HttpGet("courses")]
    [OutputCache(Tags = ["courses"])]
    public async Task<ActionResult<PaginatedList<CourseDto>>> GetCourses([FromQuery] GetAllCoursesQuery query)
    {
        var result = await sender.Send(query);
        return result.Match(Ok, Problem);
    }

    [EndpointSummary("Retrieves course counts.")]
    [EndpointDescription("Returns instructor or student course counts based on the provided query parameter.")]
    [EndpointName("GetCoursesCount")]
    [HttpGet("courses/count")]
    [OutputCache(Tags = ["courses"])]
    public async Task<ActionResult<CoursesCountDto>> GetCoursesCount(
        [FromQuery] Guid? instructorId = null,
        [FromQuery] Guid? studentId = null)
    {
        if (instructorId.HasValue)
        {
            var result = await sender.Send(new GetInstructorCoursesCountQuery(instructorId.Value));
            return result.Match(Ok, Problem);
        }

        if (studentId.HasValue)
        {
            var result = await sender.Send(new GetStudentCoursesCountQuery(studentId.Value));
            return result.Match(Ok, Problem);
        }

        return BadRequest("Either instructorId or studentId must be provided.");
    }

    [HttpGet("courses/{id:guid}")]
    [OutputCache(Tags = ["courses"])]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves a course by its unique identifier.")]
    [EndpointDescription("Fetches the details of a specific course using its ID.")]
    [EndpointName("GetCourseById")]
    public async Task<ActionResult<CourseDto>> GetCourseById(Guid id)
    {
        var result = await sender.Send(new GetCourseByIdQuery(id));
        return result.Match(Ok, Problem);
    }

    [HttpDelete("courses/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Deletes a course by its unique identifier.")]
    [EndpointDescription("Removes a course from the database using its ID.")]
    [EndpointName("DeleteCourseById")]
    public async Task<ActionResult> DeleteCourseById(Guid id)
    {
        var result = await sender.Send(new DeleteCourseByIdCommand(id));
        return result.Match(_ => NoContent(), Problem);
    }
}
