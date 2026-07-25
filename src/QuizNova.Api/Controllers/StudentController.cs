using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Mappers;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Students.Commands.DeleteStudent;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Application.Features.Students.Queries.GetAllStudents;
using QuizNova.Application.Features.Students.Queries.GetStudentById;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("students")]
[Authorize(Roles = nameof(UserRole.Admin))]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class StudentController(ISender sender) : ApiController
{
    [EndpointSummary("Retrieves all students.")]
    [EndpointDescription("Returns a paginated and filterable list of student users.")]
    [EndpointName("GetAllStudents")]
    [OutputCache(Tags = ["students"])]
    [HttpGet]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedList<StudentDto>>> GetAllStudents([FromQuery] GetAllStudentsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves a student by id.")]
    [EndpointDescription("Fetches a single student using the provided student identifier.")]
    [EndpointName("GetStudentById")]
    [OutputCache(Tags = ["students"])]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDto>> GetStudentById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetStudentByIdQuery(id), ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Creates a new student.")]
    [EndpointDescription("Creates a student account from the submitted request payload.")]
    [EndpointName("CreateStudent")]
    [HttpPost]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] CreateStudentRequest request, CancellationToken ct)
    {
        var command = request.ToCommand();

        var result = await sender.Send(command, ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Updates an existing student.")]
    [EndpointDescription("Updates profile and credential fields for the specified student.")]
    [EndpointName("UpdateStudent")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StudentDto>> UpdateStudent([FromRoute] Guid id, [FromBody] UpdateStudentRequest request, CancellationToken ct)
    {
        var command = request.ToCommand(id);

        var result = await sender.Send(command, ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Deletes a student.")]
    [EndpointDescription("Removes the student account identified by the provided student identifier.")]
    [EndpointName("DeleteStudent")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStudent([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteStudentCommand(id), ct);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}
