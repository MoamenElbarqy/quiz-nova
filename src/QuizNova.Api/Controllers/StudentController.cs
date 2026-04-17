using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizNova.Application.Features.Quizzes.Queries.GetStudentQuizzes;
using QuizNova.Application.Features.Students.Queries.GetAllStudents;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("students")]
[Authorize]
public sealed class StudentController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllStudents()
    {
        var result = await sender.Send(new GetAllStudentsQuery());

        return result.Match(
            Ok,
            Problem);
    }

    [HttpGet("{studentId:guid}/quizzes")]
    public async Task<IActionResult> GetStudentQuizzes([FromRoute] Guid studentId)
    {
        var result = await sender.Send(new GetStudentQuizzesQuery(studentId));

        return result.Match(
            Ok,
            Problem);
    }
}
