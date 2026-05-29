using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Mappers;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Courses.Commands.DeleteCourseById;
using QuizNova.Application.Features.Courses.Commands.EnrollStudentInCourse;
using QuizNova.Application.Features.Courses.Commands.RemoveStudentFromCourse;
using QuizNova.Application.Features.Courses.Commands.UpdateCourseInstructor;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Application.Features.Courses.Queries.GetAllCourses;
using QuizNova.Application.Features.Courses.Queries.GetCourseById;
using QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesCount;
using QuizNova.Application.Features.Courses.Queries.GetStudentEnrollmentsCount;
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
    [OutputCache(Tags = ["courses"])]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
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
            var result = await sender.Send(new GetStudentEnrollmentsCountQuery(studentId.Value));
            return result.Match(Ok, Problem);
        }

        return BadRequest("Either instructorId or studentId must be provided.");
    }

    [HttpGet("courses/{id:guid}")]
    [OutputCache(Tags = ["courses"])]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves a course by its unique identifier.")]
    [EndpointDescription("Fetches the details of a specific course using its ID.")]
    [EndpointName("GetCourseById")]
    public async Task<ActionResult<CourseDto>> GetCourseById(Guid id)
    {
        var result = await sender.Send(new GetCourseByIdQuery(id));
        return result.Match(Ok, Problem);
    }

    [HttpPost("courses")]
    [Authorize(Roles = nameof(UserRole.Instructor))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Creates a course.")]
    [EndpointDescription("Creates a course with an optional instructor assignment.")]
    [EndpointName("CreateCourse")]
    public async Task<ActionResult<CourseDto>> CreateCourse([FromBody] CreateCourseRequest request)
    {
        var command = request.ToCommand();

        var result = await sender.Send(command);
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
        [FromBody] UpdateCourseInstructorRequest request)
    {
        var result = await sender.Send(new UpdateCourseInstructorCommand(courseId, request.InstructorId));
        return result.Match(Ok, Problem);
    }

    [HttpPost("courses/{courseId:guid}/students/{studentId:guid}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Enrolls a student in a course.")]
    [EndpointDescription("Creates a course enrollment for the specified student.")]
    [EndpointName("EnrollStudentInCourse")]
    public async Task<ActionResult> EnrollStudentInCourse(
        Guid courseId,
        Guid studentId,
        [FromBody] EnrollStudentInCourseRequest request)
    {
        var result = await sender.Send(new EnrollStudentInCourseCommand(courseId, studentId));
        return result.Match(_ => NoContent(), Problem);
    }

    [HttpDelete("courses/{courseId:guid}/students/{studentId:guid}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Removes a student from a course.")]
    [EndpointDescription("Deletes a course enrollment for the specified student.")]
    [EndpointName("RemoveStudentFromCourse")]
    public async Task<ActionResult> RemoveStudentFromCourse(Guid courseId, Guid studentId)
    {
        var result = await sender.Send(new RemoveStudentFromCourseCommand(courseId, studentId));
        return result.Match(_ => NoContent(), Problem);
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
    public async Task<ActionResult> DeleteCourseById(Guid id)
    {
        var result = await sender.Send(new DeleteCourseByIdCommand(id));
        return result.Match(_ => NoContent(), Problem);
    }
}
