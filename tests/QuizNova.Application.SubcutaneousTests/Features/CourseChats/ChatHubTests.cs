using System.Text.Json;

using FluentAssertions;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using NSubstitute;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Hubs;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.CourseChats.Commands.ReactToMessage;
using QuizNova.Application.Features.CourseChats.Commands.RemoveReaction;
using QuizNova.Application.Features.CourseChats.Commands.SendMessage;
using QuizNova.Application.Features.CourseChats.DTOs;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.CourseChats;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.SubcutaneousTests.Features.CourseChats;

public class ChatHubTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task JoinRoom_ShouldReturnSuccess_WhenUserHasPermissionToJoin()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();

        var student = await mongoContext.Users.Find(u => u.UserRole == UserRole.Student).FirstAsync();
        var instructor = await mongoContext.Users.Find(u => u.UserRole == UserRole.Instructor).FirstAsync();
        var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100).Value;
        await mongoContext.Courses.InsertOneAsync(course);

        var room = CourseChatRoom.Create(course.Id, instructor.Id).Value;
        room.AddStudent(student.Id);
        await mongoContext.CourseChatRooms.InsertOneAsync(room);

        var mockUser = Substitute.For<IUser>();
        mockUser.Id.Returns(student.Id.ToString());

        var mockMediator = Substitute.For<IMediator>();

        var hubContext = Substitute.For<HubCallerContext>();
        hubContext.ConnectionId.Returns("conn-1");

        var groups = Substitute.For<IGroupManager>();

        var hub = new ChatHub(mongoContext, mockUser, mockMediator)
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
        var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();

        var instructor = await mongoContext.Users.Find(u => u.UserRole == UserRole.Instructor).FirstAsync();
        var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100).Value;
        await mongoContext.Courses.InsertOneAsync(course);

        var room = CourseChatRoom.Create(course.Id, instructor.Id).Value;
        await mongoContext.CourseChatRooms.InsertOneAsync(room);

        var randomUserId = Guid.NewGuid();

        var mockUser = Substitute.For<IUser>();
        mockUser.Id.Returns(randomUserId.ToString());

        var mockMediator = Substitute.For<IMediator>();

        var hubContext = Substitute.For<HubCallerContext>();
        var hub = new ChatHub(mongoContext, mockUser, mockMediator)
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
    public async Task SendMessage_ShouldDelegateToMediatorAndBroadcast()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();

        var instructor = await mongoContext.Users.Find(u => u.UserRole == UserRole.Instructor).FirstAsync();
        var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100).Value;
        await mongoContext.Courses.InsertOneAsync(course);

        var room = CourseChatRoom.Create(course.Id, instructor.Id).Value;
        await mongoContext.CourseChatRooms.InsertOneAsync(room);

        var mockUser = Substitute.For<IUser>();
        mockUser.Id.Returns(instructor.Id.ToString());

        var mockMediator = Substitute.For<IMediator>();

        var content = JsonDocument.Parse("{\"text\":\"hello world\"}");
        var expectedDto = new MessageDto(
            Guid.NewGuid(),
            room.Id,
            null!,
            null,
            DateTimeOffset.UtcNow,
            content,
            []);

        mockMediator.Send(Arg.Is<SendMessageCommand>(c => c.RoomId == room.Id))
            .Returns(expectedDto);

        var hubContext = Substitute.For<HubCallerContext>();
        var clients = Substitute.For<IHubCallerClients>();
        var clientProxy = Substitute.For<IClientProxy>();
        clients.Group(room.Id.ToString()).Returns(clientProxy);

        var hub = new ChatHub(mongoContext, mockUser, mockMediator)
        {
            Context = hubContext,
            Clients = clients,
        };

        var input = SendMessageRequest.Create(null, content);

        // Act
        var result = await hub.SendMessage(room.Id, input);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await clientProxy.Received(1).SendCoreAsync(
            "ReceiveMessage",
            Arg.Is<object[]>(args => args.Length == 1));
    }

    [Fact]
    public async Task ReactToMessage_ShouldDelegateToMediatorAndBroadcast()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();

        var student = await mongoContext.Users.Find(u => u.UserRole == UserRole.Student).FirstAsync();
        var instructor = await mongoContext.Users.Find(u => u.UserRole == UserRole.Instructor).FirstAsync();
        var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100).Value;
        await mongoContext.Courses.InsertOneAsync(course);

        var room = CourseChatRoom.Create(course.Id, instructor.Id).Value;
        room.AddStudent(student.Id);

        var content = JsonDocument.Parse("{\"text\":\"hello world\"}");
        var message = room.SendMessage(instructor.Id, null, content).Value;
        await mongoContext.CourseChatRooms.InsertOneAsync(room);

        var mockUser = Substitute.For<IUser>();
        mockUser.Id.Returns(student.Id.ToString());

        var mockMediator = Substitute.For<IMediator>();
        var expectedDto = new ReactDto(Guid.NewGuid(), message.Id, student.Id, "👍", DateTimeOffset.UtcNow);

        mockMediator.Send(Arg.Is<ReactToMessageCommand>(c => c.RoomId == room.Id && c.MessageId == message.Id))
            .Returns(expectedDto);

        var hubContext = Substitute.For<HubCallerContext>();
        var clients = Substitute.For<IHubCallerClients>();
        var clientProxy = Substitute.For<IClientProxy>();
        clients.Group(room.Id.ToString()).Returns(clientProxy);

        var hub = new ChatHub(mongoContext, mockUser, mockMediator)
        {
            Context = hubContext,
            Clients = clients,
        };

        var input = ReactOnAMessageRequest.Create(message.Id, "👍");

        // Act
        var result = await hub.ReactToMessage(room.Id, input);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await clientProxy.Received(1).SendCoreAsync(
            "ReceiveReaction",
            Arg.Is<object[]>(args => args.Length == 1));
    }

    [Fact]
    public async Task RemoveReaction_ShouldDelegateToMediatorAndBroadcast()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();

        var student = await mongoContext.Users.Find(u => u.UserRole == UserRole.Student).FirstAsync();
        var instructor = await mongoContext.Users.Find(u => u.UserRole == UserRole.Instructor).FirstAsync();
        var course = Course.Create(instructor.Id, $"Course {Guid.NewGuid().ToString()[..8]}", 50, 100).Value;
        await mongoContext.Courses.InsertOneAsync(course);

        var room = CourseChatRoom.Create(course.Id, instructor.Id).Value;
        room.AddStudent(student.Id);

        var content = JsonDocument.Parse("{\"text\":\"hello world\"}");
        var message = room.SendMessage(instructor.Id, null, content).Value;

        var reaction = Reaction.Create(message.Id, student.Id, "👍").Value;
        message.AddReaction(reaction);

        await mongoContext.CourseChatRooms.InsertOneAsync(room);

        var mockUser = Substitute.For<IUser>();
        mockUser.Id.Returns(student.Id.ToString());

        var mockMediator = Substitute.For<IMediator>();

        Result<Success> successResult = default(Success);
        mockMediator.Send(Arg.Is<RemoveReactionCommand>(
                c => c.RoomId == room.Id && c.MessageId == message.Id && c.ReactionId == reaction.Id))
            .Returns(successResult);

        var hubContext = Substitute.For<HubCallerContext>();
        var clients = Substitute.For<IHubCallerClients>();
        var clientProxy = Substitute.For<IClientProxy>();
        clients.Group(room.Id.ToString()).Returns(clientProxy);

        var hub = new ChatHub(mongoContext, mockUser, mockMediator)
        {
            Context = hubContext,
            Clients = clients,
        };

        // Act
        var result = await hub.RemoveReaction(room.Id, message.Id, reaction.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await clientProxy.Received(1).SendCoreAsync(
            "ReceiveReactionRemoved",
            Arg.Is<object[]>(args => args.Length == 1));
    }
}
