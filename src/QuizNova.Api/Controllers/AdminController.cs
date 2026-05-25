using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Admins.Commands.CreateAdmin;
using QuizNova.Application.Features.Admins.Commands.DeleteAdmin;
using QuizNova.Application.Features.Admins.Commands.UpdateAdmin;
using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Application.Features.Admins.Queries.GetAdminById;
using QuizNova.Application.Features.Admins.Queries.GetAllAdmins;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("admins")]
[Authorize(Roles = nameof(UserRole.Admin))]
public sealed class AdminController(ISender sender) : ApiController
{
    [EndpointSummary("Retrieves all admins.")]
    [EndpointDescription("Returns a paginated and filterable list of admin users.")]
    [EndpointName("GetAllAdmins")]
    [HttpGet]
    [OutputCache(Tags = ["admins"])]
    [ProducesResponseType(typeof(PaginatedList<AdminDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAdmins([FromQuery] GetAllAdminsQuery query)
    {
        var result = await sender.Send(query);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Retrieves an admin by id.")]
    [EndpointDescription("Fetches a single admin using the provided admin identifier.")]
    [EndpointName("GetAdminById")]
    [HttpGet("{id:guid}")]
    [OutputCache(Tags = ["admins"])]
    [ProducesResponseType(typeof(AdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAdminById([FromRoute] Guid id)
    {
        var result = await sender.Send(new GetAdminByIdQuery(id));

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Creates a new admin.")]
    [EndpointDescription("Creates an admin account from the submitted request payload.")]
    [EndpointName("CreateAdmin")]
    [HttpPost]
    [ProducesResponseType(typeof(AdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest request)
    {
        var command = new CreateAdminCommand(
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

    [EndpointSummary("Updates an existing admin.")]
    [EndpointDescription("Updates profile and credential fields for the specified admin.")]
    [EndpointName("UpdateAdmin")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AdminDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAdmin([FromRoute] Guid id, [FromBody] UpdateAdminRequest request)
    {
        var command = new UpdateAdminCommand(
            id,
            request.Name,
            request.Email,
            request.PhoneNumber);

        var result = await sender.Send(command);

        return result.Match(
            Ok,
            Problem);
    }

    [EndpointSummary("Deletes an admin.")]
    [EndpointDescription("Removes the admin account identified by the provided admin identifier.")]
    [EndpointName("DeleteAdmin")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAdmin([FromRoute] Guid id)
    {
        var result = await sender.Send(new DeleteAdminCommand(id));

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}
