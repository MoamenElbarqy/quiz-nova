using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Admins.Commands.CreateAdmin;
using QuizNova.Application.Features.Admins.Commands.DeleteAdmin;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.SubcutaneousTests.Features.Admins.Commands.DeleteAdmin;

public class DeleteAdminCommandHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithEmptyId_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new DeleteAdminCommand(Guid.Empty);

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
        var command = new DeleteAdminCommand(nonExistentId);

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(ApplicationErrors.AdminNotFound(Guid.Empty).Code);
    }

    [Fact]
    public async Task Handle_WithExistingId_ShouldDeleteSuccessfully()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create a valid Admin first
        var uniqueEmail = $"admin_{Guid.NewGuid()}@example.com";
        var uniquePhone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createCommand = new CreateAdminCommand("Admin to Delete", uniqueEmail, "SecurePass123!", uniquePhone,
            nameof(UserRole.Admin));
        var createResult = await mediator.Send(createCommand);
        createResult.IsSuccess.Should().BeTrue();

        var adminId = createResult.Value.Id;

        var deleteCommand = new DeleteAdminCommand(adminId);

        // Act
        var deleteResult = await mediator.Send(deleteCommand);

        // Assert
        deleteResult.IsSuccess.Should().BeTrue();

        // Verify removed from database
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var adminInDb = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
        adminInDb.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithExistingIdDeletedTwice_ShouldReturnNotFoundErrorOnSecondDelete()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create a valid Admin first
        var uniqueEmail = $"admin_{Guid.NewGuid()}@example.com";
        var uniquePhone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createCommand = new CreateAdminCommand("Admin to Delete Twice", uniqueEmail, "SecurePass123!", uniquePhone,
            nameof(UserRole.Admin));
        var createResult = await mediator.Send(createCommand);
        createResult.IsSuccess.Should().BeTrue();

        var adminId = createResult.Value.Id;

        var deleteCommand1 = new DeleteAdminCommand(adminId);
        var deleteCommand2 = new DeleteAdminCommand(adminId);

        // Act
        var deleteResult1 = await mediator.Send(deleteCommand1);
        var deleteResult2 = await mediator.Send(deleteCommand2);

        // Assert
        deleteResult1.IsSuccess.Should().BeTrue();
        deleteResult2.IsError.Should().BeTrue();
        deleteResult2.TopError.Code.Should().Be(ApplicationErrors.AdminNotFound(Guid.Empty).Code);
    }
}
