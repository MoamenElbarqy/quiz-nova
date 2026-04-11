using MediatR;

using Microsoft.AspNetCore.Mvc;

using QuizNova.Application.Features.Instructor.Queries.GetCollegeInstructors;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("instructors")]
public class InstructorController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetCollegeInstructors()
    {
        var result = await sender.Send(new GetCollegeInstructorsQuery());

        return result.Match(
            Ok,
            Problem);
    }
}
