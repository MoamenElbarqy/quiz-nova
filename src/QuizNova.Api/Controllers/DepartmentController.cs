using MediatR;

using Microsoft.AspNetCore.Mvc;

using QuizNova.Application.Features.Departments.Queries.GetCollegeDepartments;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("departments")]
public sealed class DepartmentController(ISender sender) : ApiController
{
    [HttpGet("/colleges/departments")]
    public async Task<IActionResult> GetCollegeDepartments()
    {
        var result = await sender.Send(new GetAllDepartmentsQuery());

        return result.Match(
            Ok,
            Problem);
    }
}
