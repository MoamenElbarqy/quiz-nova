using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Instructor.Commands.CreateInstructor;
using QuizNova.Application.Features.Instructor.Commands.DeleteInstructor;
using QuizNova.Application.Features.Instructor.Commands.UpdateInstructor;
using QuizNova.Application.Features.Instructor.Queries.GetAllInstructors;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("instructors")]
[Authorize]
public sealed class InstructorController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllInstructors()
    {
        var result = await sender.Send(new GetAllInstructorsQuery());

        return result.Match(
            Ok,
            Problem);
    }

    [HttpPost]
    public async Task<IActionResult> CreateInstructor([FromBody] CreateInstructorRequest request)
    {
        var command = new CreateInstructorCommand(
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

    [HttpPut("{instructorId:guid}")]
    public async Task<IActionResult> UpdateInstructor(
        [FromRoute] Guid instructorId,
        [FromBody] UpdateInstructorRequest request)
    {
        var command = new UpdateInstructorCommand(
            instructorId,
            request.Name,
            request.Email,
            request.Password,
            request.PhoneNumber);

        var result = await sender.Send(command);

        return result.Match(
            Ok,
            Problem);
    }

    [HttpDelete("{instructorId:guid}")]
    public async Task<IActionResult> DeleteInstructor([FromRoute] Guid instructorId)
    {
        var result = await sender.Send(new DeleteInstructorCommand(instructorId));

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}
