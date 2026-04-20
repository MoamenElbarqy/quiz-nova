using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizNova.Application.Features.Courses.Queries.GetAllCourses;
using QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesById;
using QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesCount;
using QuizNova.Application.Features.Courses.Queries.GetStudentCoursesCount;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Api.Controllers;

[ApiController]
[Authorize]
public sealed class CourseController(ISender sender) : ApiController
{
    [HttpGet("courses")]
    public async Task<IActionResult> GetCourses([FromQuery] Guid? instructorId = null)
    {
        if (instructorId.HasValue)
        {
            var instructorCoursesResult = await sender.Send(new GetInstructorCoursesByIdQuery(instructorId.Value));
            return instructorCoursesResult.Match(
                Ok,
                Problem);
        }

        var result = await sender.Send(new GetAllCoursesQuery());
        return result.Match(
            Ok,
            Problem);
    }

    [HttpGet("courses/count")]
    public async Task<IActionResult> GetCoursesCount(
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
}
