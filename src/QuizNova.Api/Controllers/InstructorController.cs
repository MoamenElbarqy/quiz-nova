using MediatR;

using Microsoft.AspNetCore.Mvc;

using QuizNova.Application.Features.Instructor.Queries.GetAllInstructors;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("instructors")]
public class InstructorController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllInstructors()
    {
        var result = await sender.Send(new GetAllInstructorsQuery());

        return result.Match(
            Ok,
            Problem);
    }
}
