using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Instructors.Commands.CreateInstructor;
using QuizNova.Application.Features.Instructors.Commands.DeleteInstructor;
using QuizNova.Application.Features.Instructors.Commands.UpdateInstructor;
using QuizNova.Application.Features.Instructors.Queries.GetAllInstructors;
using QuizNova.Application.Features.Instructors.Queries.GetInstructorById;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("instructors")]
[Authorize]
public sealed class InstructorController(ISender sender) : ApiController
{
    [EndpointSummary("Retrieves all instructors.")]
    [EndpointDescription("Returns a paginated and filterable list of instructor users.")]
    [EndpointName("GetAllInstructors")]
    [HttpGet]
    [OutputCache(Tags = ["instructors"])]
    public async Task<IActionResult> GetAllInstructors([FromQuery] GetAllInstructorsQuery query)
    {
        var result = await sender.Send(query);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves an instructor by id.")]
    [EndpointDescription("Fetches a single instructor using the provided instructor identifier.")]
    [EndpointName("GetInstructorById")]
    [HttpGet("{id:guid}")]
    [OutputCache(Tags = ["instructors"])]
    public async Task<IActionResult> GetInstructorById([FromRoute] Guid id)
    {
        var result = await sender.Send(new GetInstructorByIdQuery(id));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Creates a new instructor.")]
    [EndpointDescription("Creates an instructor account from the submitted request payload.")]
    [EndpointName("CreateInstructor")]
    [HttpPost]
    public async Task<IActionResult> CreateInstructor([FromBody] CreateInstructorRequest request)
    {
        var command = new CreateInstructorCommand(
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

    [EndpointSummary("Updates an existing instructor.")]
    [EndpointDescription("Updates profile and credential fields for the specified instructor.")]
    [EndpointName("UpdateInstructor")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateInstructor(
        [FromRoute] Guid id,
        [FromBody] UpdateInstructorRequest request)
    {
        var command = new UpdateInstructorCommand(
            id,
            request.Name,
            request.Email,
            request.PhoneNumber);

        var result = await sender.Send(command);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Deletes an instructor.")]
    [EndpointDescription("Removes the instructor account identified by the provided instructor identifier.")]
    [EndpointName("DeleteInstructor")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteInstructor([FromRoute] Guid id)
    {
        var result = await sender.Send(new DeleteInstructorCommand(id));

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}
