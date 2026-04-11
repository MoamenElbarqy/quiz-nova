using MediatR;

using Microsoft.AspNetCore.Mvc;

using QuizNova.Application.Features.Students.Queries.GetCollegeStudents;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("students")]
public sealed class StudentController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetCollegeStudents()
    {
        var result = await sender.Send(new GetAllStudentsQuery());

        return result.Match(
            Ok,
            Problem);
    }
}
