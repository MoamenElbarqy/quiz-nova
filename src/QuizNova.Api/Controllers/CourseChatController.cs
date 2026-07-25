using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Application.Features.CourseChats.Queries.GetCourseChatRoomByCourseId;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Controllers;

[ApiController]
[Authorize(Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Instructor)}")]
[Route("")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class CourseChatController(ISender sender) : ApiController
{
    [HttpGet("courses/{courseId:guid}/chatroom")]
    [ProducesResponseType(typeof(CourseChatRoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Retrieves the chatroom data for a course.")]
    [EndpointDescription("Returns the chatroom status, student IDs, and message history for a specific course.")]
    [EndpointName("GetChatRoomData")]
    public async Task<ActionResult<CourseChatRoomDto>> GetChatRoomData(Guid courseId, CancellationToken ct)
    {
        var result = await sender.Send(new GetCourseChatRoomByCourseIdQuery(courseId), ct);
        return result.Match(Ok, Problem);
    }
}
