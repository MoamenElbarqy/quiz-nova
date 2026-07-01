using System.Net;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using QuizNova.Api.IntegrationTests.Common;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Tests.Common.Security;

using Xunit;

namespace QuizNova.Api.IntegrationTests.Controllers;

public class CourseChatControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetChatRoomData_WhenUnauthenticated_ReturnsUnauthorized()
    {
        using var client = factory.CreateAppHttpClient();

        var response = await client.GetAsync($"/courses/{Guid.NewGuid()}/chatroom");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetChatRoomData_WhenAdmin_ReturnsForbidden()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        var response = await client.GetAsync($"/courses/{Guid.NewGuid()}/chatroom");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetChatRoomData_WhenInstructorOfCourse_ReturnsOk()
    {
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");

        Guid courseId;
        Guid instructorId;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var course = await dbContext.Courses.FirstAsync();
            courseId = course.Id;
            instructorId = course.InstructorId!.Value;
        }

        // Retrieve course chatroom
        var response = await client.GetAsync($"/courses/{courseId}/chatroom");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var chatRoom = await response.Content.ReadFromJsonAsync<CourseChatRoomDto>();
        chatRoom.Should().NotBeNull();
        chatRoom.CourseId.Should().Be(courseId);
        chatRoom.InstructorId.Should().Be(instructorId);
    }
}
