using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.CourseChats.Queries.GetCourseChatRoomByCourseId;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Infrastructure.Identity;
using QuizNova.Tests.Common.Security;

namespace QuizNova.Application.SubcutaneousTests.Features.CourseChats.Queries.GetCourseChatRoomByCourseId;

public class GetCourseChatRoomByCourseIdQueryTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithUnauthorizedUser_ShouldReturnForbidden()
    {
        var mediator = factory.CreateMediator();

        Guid courseId;
        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
            var course = await mongoContext.Courses.Find(_ => true).FirstAsync();
            courseId = course.Id;
        }

        // Set current user to random Guid not in the chat room
        TestCurrentUser.Set(new AppUser { Id = Guid.NewGuid().ToString() });

        var query = new GetCourseChatRoomByCourseIdQuery(courseId);

        var result = await mediator.Send(query);

        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("CourseChatRoom.CannotJoin");
    }

    [Fact]
    public async Task Handle_WithInstructor_ShouldReturnCourseChatRoomDto()
    {
        var mediator = factory.CreateMediator();

        Guid courseId;
        Guid instructorId;
        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
            var course = await mongoContext.Courses.Find(_ => true).FirstAsync();
            courseId = course.Id;
            instructorId = course.InstructorId!.Value;
        }

        // Set current user to instructor
        TestCurrentUser.Set(new AppUser { Id = instructorId.ToString() });

        var query = new GetCourseChatRoomByCourseIdQuery(courseId);

        var result = await mediator.Send(query);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.CourseId.Should().Be(courseId);
        result.Value.InstructorId.Should().Be(instructorId);
    }
}
