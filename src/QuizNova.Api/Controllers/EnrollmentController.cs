using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Common.Caching;
using QuizNova.Application.Features.Enrollments.Commands.DisenrollStudentFromCourse;
using QuizNova.Application.Features.Enrollments.Commands.EnrollStudentInCourse;
using QuizNova.Application.Features.Enrollments.DTOs;
using QuizNova.Application.Features.Enrollments.Queries.GetAllCoursesEnrollmentCount;
using QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsById;
using QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsCount;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Controllers;

[ApiController]
[Authorize]
[Route("")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class EnrollmentController(ISender sender) : ApiController
{
    [HttpPost("students/{studentId:guid}/enrollments")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Enrolls a student in a course.")]
    [EndpointDescription("Creates a course enrollment for the specified student.")]
    [EndpointName("EnrollStudentInCourse")]
    public async Task<ActionResult> EnrollStudentInCourse(Guid studentId,
        [FromBody] EnrollStudentInCourseRequest request,
        CancellationToken ct)
    {
        var result = await sender.Send(new EnrollStudentInCourseCommand(request.CourseId, studentId), ct);
        return result.Match(_ => NoContent(), Problem);
    }

    [HttpDelete("students/{studentId:guid}/enrollments/{enrollmentId:guid}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Disenrolls a student from a course.")]
    [EndpointDescription("Disenrolls a student from a course.")]
    [EndpointName("DisenrollStudentFromCourse")]
    public async Task<ActionResult> DisenrollStudentFromCourse(Guid enrollmentId, Guid studentId, CancellationToken ct)
    {
        var result = await sender.Send(new DisenrollStudentFromCourseCommand(enrollmentId, studentId), ct);
        return result.Match(_ => NoContent(), Problem);
    }

    [HttpGet("students/{studentId:guid}/enrollments")]
    [OutputCache(Tags = [CacheTags.Enrollments])]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves student enrollments.")]
    [EndpointDescription("Returns the enrollments for a specific student.")]
    [EndpointName("GetStudentEnrollments")]
    public async Task<ActionResult<List<EnrollmentDto>>> GetStudentEnrollments(Guid studentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetStudentEnrollmentsByIdQuery(studentId), ct);
        return result.Match(Ok, Problem);
    }

    [HttpGet("students/{studentId:guid}/enrollments/count")]
    [OutputCache(Tags = [CacheTags.Enrollments])]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves student enrollment counts.")]
    [EndpointDescription("Returns student enrollment count based on the student ID.")]
    [EndpointName("GetStudentEnrollmentsCount")]
    public async Task<ActionResult<EnrollmentCountDto>> GetStudentEnrollmentsCount(Guid studentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetStudentEnrollmentsCountQuery(studentId), ct);
        return result.Match(Ok, Problem);
    }

    [HttpGet("courses/enrollments/count")]
    [OutputCache(Tags = [CacheTags.Courses, CacheTags.Enrollments])]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [ProducesResponseType(typeof(List<CourseEnrollmentCountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Retrieves enrollment counts for all courses.")]
    [EndpointDescription("Returns a list of all courses with their enrollment count, sorted descending. Admin only.")]
    [EndpointName("GetAllCoursesEnrollmentCount")]
    public async Task<ActionResult<List<CourseEnrollmentCountDto>>> GetAllCoursesEnrollmentCount(CancellationToken ct)
    {
        var result = await sender.Send(new GetAllCoursesEnrollmentCountQuery(), ct);
        return result.Match(Ok, Problem);
    }
}
