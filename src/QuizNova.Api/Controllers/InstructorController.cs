using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Mappers;
using QuizNova.Application.Common.Caching;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Instructors.Commands.DeleteInstructor;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Application.Features.Instructors.Queries.GetAllInstructors;
using QuizNova.Application.Features.Instructors.Queries.GetInstructorById;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("instructors")]
[Authorize(Roles = nameof(UserRole.Admin))]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class InstructorController(ISender sender) : ApiController
{
    [EndpointSummary("Retrieves all instructors.")]
    [EndpointDescription("Returns a paginated and filterable list of instructor users.")]
    [EndpointName("GetAllInstructors")]
    [HttpGet]
    [OutputCache(Tags = [CacheTags.Instructors])]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedList<InstructorDto>>> GetAllInstructors([FromQuery] GetAllInstructorsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves an instructor by id.")]
    [EndpointDescription("Fetches a single instructor using the provided instructor identifier.")]
    [EndpointName("GetInstructorById")]
    [HttpGet("{id:guid}")]
    [OutputCache(Tags = [CacheTags.Instructors])]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InstructorDto>> GetInstructorById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetInstructorByIdQuery(id), ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Creates a new instructor.")]
    [EndpointDescription("Creates an instructor account from the submitted request payload.")]
    [EndpointName("CreateInstructor")]
    [HttpPost]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<InstructorDto>> CreateInstructor([FromBody] CreateInstructorRequest request, CancellationToken ct)
    {
        var command = request.ToCommand();

        var result = await sender.Send(command, ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Updates an existing instructor.")]
    [EndpointDescription("Updates profile and credential fields for the specified instructor.")]
    [EndpointName("UpdateInstructor")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<InstructorDto>> UpdateInstructor(
        [FromRoute] Guid id,
        [FromBody] UpdateInstructorRequest request,
        CancellationToken ct)
    {
        var command = request.ToCommand(id);

        var result = await sender.Send(command, ct);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Deletes an instructor.")]
    [EndpointDescription("Removes the instructor account identified by the provided instructor identifier.")]
    [EndpointName("DeleteInstructor")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInstructor([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteInstructorCommand(id), ct);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}
