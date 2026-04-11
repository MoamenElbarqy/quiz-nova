using MediatR;

using Microsoft.AspNetCore.Mvc;

using QuizNova.Application.Features.Courses.Queries.GetAllCourses;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("courses")]
public class CourseController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllCourses()
    {
        var result = await sender.Send(new GetAllCoursesQuery());

        return result.Match(
            Ok,
            Problem);
    }
}
