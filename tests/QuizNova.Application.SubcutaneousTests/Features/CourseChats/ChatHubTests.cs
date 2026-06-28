using System.Text.Json;

using FluentAssertions;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using NSubstitute;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Hubs;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.CourseChats;
using QuizNova.Domain.Entities.Courses;

namespace QuizNova.Application.SubcutaneousTests.Features.CourseChats;

public class ChatHubTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task JoinRoom_ShouldReturnSuccess_WhenUserHasPermissionToJoin()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var student = await dbContext.Students.FirstAsync();
        var instructor = await dbContext.Instructors.FirstAsync();
        var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100, [], []).Value;
        await dbContext.Courses.AddAsync(course);

        // Create CourseChatRoom and add student
        var room = CourseChatRoom.Create(course.Id, instructor.Id).Value;
        room.AddStudent(student);
        await dbContext.CourseChatRooms.AddAsync(room);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Mock IUser and SignalR plumbing
        var mockUser = Substitute.For<IUser>();
        mockUser.Id.Returns(student.Id.ToString());

        var hubContext = Substitute.For<HubCallerContext>();
        hubContext.ConnectionId.Returns("conn-1");

        var groups = Substitute.For<IGroupManager>();

        var hub = new ChatHub(dbContext, mockUser)
        {
            Context = hubContext,
            Groups = groups,
        };

        // Act
        var result = await hub.JoinRoom(room.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await groups.Received(1).AddToGroupAsync("conn-1", room.Id.ToString());
    }

    [Fact]
    public async Task JoinRoom_ShouldReturnForbidden_WhenUserIsNotEnrolledOrInstructor()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var instructor = await dbContext.Instructors.FirstAsync();
        var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100, [], []).Value;
        await dbContext.Courses.AddAsync(course);

        var room = CourseChatRoom.Create(course.Id, instructor.Id).Value;
        await dbContext.CourseChatRooms.AddAsync(room);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // A random user Guid who is not enrolled
        var randomUserId = Guid.NewGuid();

        var mockUser = Substitute.For<IUser>();
        mockUser.Id.Returns(randomUserId.ToString());

        var hubContext = Substitute.For<HubCallerContext>();
        var hub = new ChatHub(dbContext, mockUser)
        {
            Context = hubContext,
        };

        // Act
        var result = await hub.JoinRoom(room.Id);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("CourseChatRoom.CannotJoin");
    }

    [Fact]
    public async Task SendMessage_ShouldReturnSuccessAndBroadcast_WhenUserCanSend()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var instructor = await dbContext.Instructors.FirstAsync();
        var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100, [], []).Value;
        await dbContext.Courses.AddAsync(course);

        var room = CourseChatRoom.Create(course.Id, instructor.Id).Value;
        await dbContext.CourseChatRooms.AddAsync(room);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var mockUser = Substitute.For<IUser>();
        mockUser.Id.Returns(instructor.Id.ToString());

        var hubContext = Substitute.For<HubCallerContext>();
        var clients = Substitute.For<IHubCallerClients>();
        var clientProxy = Substitute.For<IClientProxy>();
        clients.Group(room.Id.ToString()).Returns(clientProxy);

        var hub = new ChatHub(dbContext, mockUser)
        {
            Context = hubContext,
            Clients = clients,
        };

        var content = JsonDocument.Parse("{\"text\":\"hello world\"}");
        var input = SendMessageRequest.Create(null, content);

        // Act
        var result = await hub.SendMessage(room.Id, input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Content.RootElement.GetProperty("text").GetString().Should().Be("hello world");

        // Verify message was saved to DB
        var savedMessage = await dbContext.CourseChatRoomMessages
            .FirstOrDefaultAsync(m => m.RoomId == room.Id && m.SenderId == instructor.Id);
        savedMessage.Should().NotBeNull();

        // Verify broadcast
        await clientProxy.Received(1).SendCoreAsync(
            "ReceiveMessage",
            Arg.Is<object[]>(args => args.Length == 1));
    }

    [Fact]
    public async Task ReactToMessage_ShouldReturnSuccessAndBroadcast_WhenUserCanReact()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var student = await dbContext.Students.FirstAsync();
        var instructor = await dbContext.Instructors.FirstAsync();
        var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100, [], []).Value;
        await dbContext.Courses.AddAsync(course);

        var room = CourseChatRoom.Create(course.Id, instructor.Id).Value;
        room.AddStudent(student);
        await dbContext.CourseChatRooms.AddAsync(room);

        var content = JsonDocument.Parse("{\"text\":\"hello world\"}");
        var message = Message.Create(room.Id, instructor.Id, null, content).Value;
        await dbContext.CourseChatRoomMessages.AddAsync(message);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        ((DbContext)dbContext).ChangeTracker.Clear();

        var mockUser = Substitute.For<IUser>();
        mockUser.Id.Returns(student.Id.ToString());

        var hubContext = Substitute.For<HubCallerContext>();
        var clients = Substitute.For<IHubCallerClients>();
        var clientProxy = Substitute.For<IClientProxy>();
        clients.Group(room.Id.ToString()).Returns(clientProxy);

        var hub = new ChatHub(dbContext, mockUser)
        {
            Context = hubContext,
            Clients = clients,
        };

        var input = ReactOnAMessageRequest.Create(message.Id, "👍");

        // Act
        var result = await hub.ReactToMessage(room.Id, input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Emoji.Should().Be("👍");
        result.Value.MessageId.Should().Be(message.Id);
        result.Value.ReactorId.Should().Be(student.Id);

        // Verify reaction was saved to DB
        var savedMessage = await dbContext.CourseChatRoomMessages
            .Include(m => m.Reacts)
            .FirstOrDefaultAsync(m => m.Id == message.Id);
        savedMessage.Should().NotBeNull();
        savedMessage.Reacts.Should().ContainSingle(r => r.ReactorId == student.Id && r.Emoji == "👍");

        // Verify broadcast
        await clientProxy.Received(1).SendCoreAsync(
            "ReceiveReaction",
            Arg.Is<object[]>(args => args.Length == 1));
    }

    [Fact]
    public async Task RemoveReaction_ShouldReturnSuccessAndBroadcast_WhenUserCanRemoveReaction()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var student = await dbContext.Students.FirstAsync();
        var instructor = await dbContext.Instructors.FirstAsync();
        var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100, [], []).Value;
        await dbContext.Courses.AddAsync(course);

        var room = CourseChatRoom.Create(course.Id, instructor.Id).Value;
        room.AddStudent(student);
        await dbContext.CourseChatRooms.AddAsync(room);

        var content = JsonDocument.Parse("{\"text\":\"hello world\"}");
        var message = Message.Create(room.Id, instructor.Id, null, content).Value;

        var react = React.Create(message.Id, student.Id, "👍").Value;
        message.AddReaction(react);

        await dbContext.CourseChatRoomMessages.AddAsync(message);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        ((DbContext)dbContext).ChangeTracker.Clear();

        var mockUser = Substitute.For<IUser>();
        mockUser.Id.Returns(student.Id.ToString());

        var hubContext = Substitute.For<HubCallerContext>();
        var clients = Substitute.For<IHubCallerClients>();
        var clientProxy = Substitute.For<IClientProxy>();
        clients.Group(room.Id.ToString()).Returns(clientProxy);

        var hub = new ChatHub(dbContext, mockUser)
        {
            Context = hubContext,
            Clients = clients,
        };

        // Act
        var result = await hub.RemoveReaction(room.Id, message.Id, react.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verify reaction was removed from DB
        var savedMessage = await dbContext.CourseChatRoomMessages
            .Include(m => m.Reacts)
            .FirstOrDefaultAsync(m => m.Id == message.Id);
        savedMessage.Should().NotBeNull();
        savedMessage.Reacts.Should().NotContain(r => r.ReactorId == student.Id && r.Emoji == "👍");

        // Verify broadcast
        await clientProxy.Received(1).SendCoreAsync(
            "ReceiveReactionRemoved",
            Arg.Is<object[]>(args => args.Length == 1));
    }
}
