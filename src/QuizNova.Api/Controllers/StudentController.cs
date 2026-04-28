using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Quizzes.Queries.GetStudentQuizzes;
using QuizNova.Application.Features.Students.Commands.CreateStudent;
using QuizNova.Application.Features.Students.Commands.DeleteStudent;
using QuizNova.Application.Features.Students.Commands.UpdateStudent;
using QuizNova.Application.Features.Students.Queries.GetAllStudents;
using QuizNova.Application.Features.Students.Queries.GetStudentById;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("students")]
[Authorize]
public sealed class StudentController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllStudents([FromQuery] GetAllStudentsQuery query)
    {
        var result = await sender.Send(query);

        return result.Match(
            Ok,
            Problem);
    }

    [HttpGet("{studentId:guid}")]
    public async Task<IActionResult> GetStudentById([FromRoute] Guid studentId)
    {
        var result = await sender.Send(new GetStudentByIdQuery(studentId));

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

    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
    {
        var command = new CreateStudentCommand(
            request.Id,
            request.Name,
            request.Email,
            request.Password,
            request.PhoneNumber,
            request.Role);

        var result = await sender.Send(command);

        return result.Match(
            Ok,
            Problem);
    }

    [HttpPut("{studentId:guid}")]
    public async Task<IActionResult> UpdateStudent([FromRoute] Guid studentId, [FromBody] UpdateStudentRequest request)
    {
        var command = new UpdateStudentCommand(
            studentId,
            request.Name,
            request.Email,
            request.Password,
            request.PhoneNumber);

        var result = await sender.Send(command);

        return result.Match(
            Ok,
            Problem);
    }

    [HttpDelete("{studentId:guid}")]
    public async Task<IActionResult> DeleteStudent([FromRoute] Guid studentId)
    {
        var result = await sender.Send(new DeleteStudentCommand(studentId));

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}
