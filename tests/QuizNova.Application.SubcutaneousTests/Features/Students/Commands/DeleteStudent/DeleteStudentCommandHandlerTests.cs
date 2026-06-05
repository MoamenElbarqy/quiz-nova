using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Students.Commands.CreateStudent;
using QuizNova.Application.Features.Students.Commands.DeleteStudent;
using QuizNova.Application.Features.Users.DTOs;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.SubcutaneousTests.Features.Students.Commands.DeleteStudent;

public class DeleteStudentCommandHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithEmptyId_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new DeleteStudentCommand(Guid.Empty);

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Id" && e.Description.Contains("required"));
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldReturnNotFoundError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var nonExistentId = Guid.NewGuid();
        var command = new DeleteStudentCommand(nonExistentId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(ApplicationErrors.StudentNotFound(Guid.Empty).Code);
    }

    [Fact]
    public async Task Handle_WithExistingId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create a valid Student first
        var uniqueEmail = $"student_{Guid.NewGuid()}@example.com";
        var uniquePhone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createCommand =
            new CreateStudentCommand(new PersonalInformationDto("Student to Delete", uniqueEmail, uniquePhone),
                "SecurePass123!", nameof(UserRole.Student));
        var createResult = await mediator.Send(createCommand);
        createResult.IsSuccess.Should().BeTrue();

        var studentId = createResult.Value.Id;

        var deleteCommand = new DeleteStudentCommand(studentId);

        // Act
        var deleteResult = await mediator.Send(deleteCommand);

        // Assert
        deleteResult.IsSuccess.Should().BeTrue();

        // Verify removed from database
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var studentInDb = await dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId);
        studentInDb.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithExistingIdDeletedTwice_ShouldReturnNotFoundErrorOnSecondDelete()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create a valid Student first
        var uniqueEmail = $"student_{Guid.NewGuid()}@example.com";
        var uniquePhone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createCommand =
            new CreateStudentCommand(new PersonalInformationDto("Student to Delete Twice", uniqueEmail, uniquePhone),
                "SecurePass123!", nameof(UserRole.Student));
        var createResult = await mediator.Send(createCommand);
        createResult.IsSuccess.Should().BeTrue();

        var studentId = createResult.Value.Id;

        var deleteCommand1 = new DeleteStudentCommand(studentId);
        var deleteCommand2 = new DeleteStudentCommand(studentId);

        // Act
        var deleteResult1 = await mediator.Send(deleteCommand1);
        var deleteResult2 = await mediator.Send(deleteCommand2);

        // Assert
        deleteResult1.IsSuccess.Should().BeTrue();
        deleteResult2.IsError.Should().BeTrue();
        deleteResult2.TopError.Code.Should().Be(ApplicationErrors.StudentNotFound(Guid.Empty).Code);
    }
}
