using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructors.Commands.CreateInstructor;
using QuizNova.Application.Features.Instructors.Commands.DeleteInstructor;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.SubcutaneousTests.Features.Instructors.Commands.DeleteInstructor;

public class DeleteInstructorCommandHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithEmptyId_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new DeleteInstructorCommand(Guid.Empty);

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
        var command = new DeleteInstructorCommand(nonExistentId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(ApplicationErrors.InstructorNotFound(Guid.Empty).Code);
    }

    [Fact]
    public async Task Handle_WithExistingId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create a valid Instructor first
        var uniqueEmail = $"instructor_{Guid.NewGuid()}@example.com";
        var uniquePhone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createCommand = new CreateInstructorCommand("Instructor to Delete", uniqueEmail, "SecurePass123!", uniquePhone, nameof(UserRole.Instructor));
        var createResult = await mediator.Send(createCommand);
        createResult.IsSuccess.Should().BeTrue();

        var instructorId = createResult.Value.Id;

        var deleteCommand = new DeleteInstructorCommand(instructorId);

        // Act
        var deleteResult = await mediator.Send(deleteCommand);

        // Assert
        deleteResult.IsSuccess.Should().BeTrue();

        // Verify removed from database
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var instructorInDb = await dbContext.Instructors.FirstOrDefaultAsync(i => i.Id == instructorId);
        instructorInDb.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithExistingIdDeletedTwice_ShouldReturnNotFoundErrorOnSecondDelete()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create a valid Instructor first
        var uniqueEmail = $"instructor_{Guid.NewGuid()}@example.com";
        var uniquePhone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createCommand = new CreateInstructorCommand("Instructor to Delete Twice", uniqueEmail, "SecurePass123!", uniquePhone, nameof(UserRole.Instructor));
        var createResult = await mediator.Send(createCommand);
        createResult.IsSuccess.Should().BeTrue();

        var instructorId = createResult.Value.Id;

        var deleteCommand1 = new DeleteInstructorCommand(instructorId);
        var deleteCommand2 = new DeleteInstructorCommand(instructorId);

        // Act
        var deleteResult1 = await mediator.Send(deleteCommand1);
        var deleteResult2 = await mediator.Send(deleteCommand2);

        // Assert
        deleteResult1.IsSuccess.Should().BeTrue();
        deleteResult2.IsError.Should().BeTrue();
        deleteResult2.TopError.Code.Should().Be(ApplicationErrors.InstructorNotFound(Guid.Empty).Code);
    }
}
