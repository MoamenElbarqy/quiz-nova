using MediatR;

using Microsoft.AspNetCore.Authorization;

using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Application.Features.CourseChats.Queries.GetCourseChatRoomByCourseId;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Endpoints;

public static class CourseChatEndpoints
{
    public static IEndpointRouteBuilder MapCourseChatEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("courses/{courseId:guid}/chatroom", async (ISender sender, Guid courseId) =>
        {
            var result = await sender.Send(new GetCourseChatRoomByCourseIdQuery(courseId));
            return result.ToOk();
        })
        .WithName("GetChatRoomData")
        .WithSummary("Retrieves the chatroom data for a course.")
        .WithDescription("Returns the chatroom status, student IDs, and message history for a specific course.")
        .RequireAuthorization(new AuthorizeAttribute { Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Instructor)}" })
        .RequireRateLimiting("Global")
        .WithTags("course-chats")
        .Produces<CourseChatRoomDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}
