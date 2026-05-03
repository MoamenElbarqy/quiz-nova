using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

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
    [EndpointSummary("Retrieves all students.")]
    [EndpointDescription("Returns a paginated and filterable list of student users.")]
    [EndpointName("GetAllStudents")]
    [OutputCache(Tags = ["students"])]
    [HttpGet]
    public async Task<IActionResult> GetAllStudents([FromQuery] GetAllStudentsQuery query)
    {
        var result = await sender.Send(query);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves a student by id.")]
    [EndpointDescription("Fetches a single student using the provided student identifier.")]
    [EndpointName("GetStudentById")]
    [OutputCache(Tags = ["students"])]
    [HttpGet("{studentId:guid}")]
    public async Task<IActionResult> GetStudentById([FromRoute] Guid studentId)
    {
        var result = await sender.Send(new GetStudentByIdQuery(studentId));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves quizzes assigned to a student.")]
    [EndpointDescription("Returns quizzes associated with the specified student identifier.")]
    [EndpointName("GetStudentQuizzes")]
    [OutputCache(Tags = ["students", "quizzes"])]
    [HttpGet("{studentId:guid}/quizzes")]
    public async Task<IActionResult> GetStudentQuizzes([FromRoute] Guid studentId)
    {
        var result = await sender.Send(new GetStudentQuizzesQuery(studentId));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Creates a new student.")]
    [EndpointDescription("Creates a student account from the submitted request payload.")]
    [EndpointName("CreateStudent")]
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

    [EndpointSummary("Updates an existing student.")]
    [EndpointDescription("Updates profile and credential fields for the specified student.")]
    [EndpointName("UpdateStudent")]
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

    [EndpointSummary("Deletes a student.")]
    [EndpointDescription("Removes the student account identified by the provided student identifier.")]
    [EndpointName("DeleteStudent")]
    [HttpDelete("{studentId:guid}")]
    public async Task<IActionResult> DeleteStudent([FromRoute] Guid studentId)
    {
        var result = await sender.Send(new DeleteStudentCommand(studentId));

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}
