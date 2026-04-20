using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Admin.Commands.CreateAdmin;
using QuizNova.Application.Features.Admin.Commands.DeleteAdmin;
using QuizNova.Application.Features.Admin.Commands.UpdateAdmin;
using QuizNova.Application.Features.Admin.Queries.GetAllAdmins;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Controllers;

[ApiController]
[Route("admins")]
[Authorize(Roles = nameof(UserRole.Admin))]
public sealed class AdminController(ISender sender) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllAdmins()
    {
        var result = await sender.Send(new GetAllAdminsQuery());

        return result.Match(
            Ok,
            Problem);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest request)
    {
        var command = new CreateAdminCommand(
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

    [HttpPut("{adminId:guid}")]
    public async Task<IActionResult> UpdateAdmin([FromRoute] Guid adminId, [FromBody] UpdateAdminRequest request)
    {
        var command = new UpdateAdminCommand(
            adminId,
            request.Name,
            request.Email,
            request.Password,
            request.PhoneNumber);

        var result = await sender.Send(command);

        return result.Match(
            Ok,
            Problem);
    }

    [HttpDelete("{adminId:guid}")]
    public async Task<IActionResult> DeleteAdmin([FromRoute] Guid adminId)
    {
        var result = await sender.Send(new DeleteAdminCommand(adminId));

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}
