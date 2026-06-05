using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Admins.Commands.CreateAdmin;
using QuizNova.Application.Features.Admins.Commands.UpdateAdmin;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.SubcutaneousTests.Features.Admins.Commands.UpdateAdmin;

public class UpdateAdminCommandHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithEmptyId_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new UpdateAdminCommand(
            Id: Guid.Empty,
            Name: "Valid Name",
            Email: "admin@example.com",
            PhoneNumber: "+123456789");

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
        var command = new UpdateAdminCommand(
            Id: nonExistentId,
            Name: "Valid Name",
            Email: "admin@example.com",
            PhoneNumber: "+123456789");

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(ApplicationErrors.AdminNotFound(Guid.Empty).Code);
    }

    [Fact]
    public async Task Handle_WithExistingId_ShouldUpdateSuccessfully()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create a valid Admin first
        var uniqueEmail1 = $"admin_{Guid.NewGuid()}@example.com";
        var uniquePhone1 = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createCommand = new CreateAdminCommand("Original Name", uniqueEmail1, "SecurePass123!", uniquePhone1, nameof(UserRole.Admin));
        var createResult = await mediator.Send(createCommand);
        createResult.IsSuccess.Should().BeTrue();

        var adminId = createResult.Value.Id;

        // 2. Prepare Update Command
        var uniqueEmail2 = $"admin_{Guid.NewGuid()}@example.com";
        var uniquePhone2 = $"+1{Guid.NewGuid().ToString()[..10]}";
        var updateCommand = new UpdateAdminCommand(
            Id: adminId,
            Name: "Updated Admin Name",
            Email: uniqueEmail2,
            PhoneNumber: uniquePhone2);

        // Act
        var updateResult = await mediator.Send(updateCommand);

        // Assert
        updateResult.IsSuccess.Should().BeTrue();
        updateResult.Value.Name.Should().Be("Updated Admin Name");
        updateResult.Value.Email.Should().Be(uniqueEmail2);

        // Verify updated in database
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var adminInDb = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);

        adminInDb.Should().NotBeNull();
        adminInDb.PersonalInformation.Name.Should().Be("Updated Admin Name");
        adminInDb.PersonalInformation.Email.Should().Be(uniqueEmail2);
        adminInDb.PersonalInformation.PhoneNumber.Should().Be(uniquePhone2);
    }

    [Fact]
    public async Task Handle_WithInvalidData_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // Create Admin
        var uniqueEmail = $"admin_{Guid.NewGuid()}@example.com";
        var uniquePhone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createCommand = new CreateAdminCommand("Original Name", uniqueEmail, "SecurePass123!", uniquePhone, nameof(UserRole.Admin));
        var createResult = await mediator.Send(createCommand);
        var adminId = createResult.Value.Id;

        // Update with name too short
        var updateCommand = new UpdateAdminCommand(
            Id: adminId,
            Name: "Ab",
            Email: uniqueEmail,
            PhoneNumber: uniquePhone);

        // Act
        var result = await mediator.Send(updateCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Name" && e.Description.Contains("at least 3 characters"));
    }

    [Fact]
    public async Task Handle_WithDuplicateEmailForAnotherUser_ShouldReturnDuplicateEmailError()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create Admin A
        var emailA = $"admin_{Guid.NewGuid()}@example.com";
        var phoneA = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createA = new CreateAdminCommand("Admin A", emailA, "SecurePass123!", phoneA, nameof(UserRole.Admin));
        var resA = await mediator.Send(createA);
        resA.IsSuccess.Should().BeTrue();

        // 2. Create Admin B
        var emailB = $"admin_{Guid.NewGuid()}@example.com";
        var phoneB = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createB = new CreateAdminCommand("Admin B", emailB, "SecurePass123!", phoneB, nameof(UserRole.Admin));
        var resB = await mediator.Send(createB);
        var adminBId = resB.Value.Id;

        // 3. Try to update Admin B's email to match Admin A
        var updateCommand = new UpdateAdminCommand(
            Id: adminBId,
            Name: "Admin B Updated",
            Email: emailA,
            PhoneNumber: phoneB);

        // Act
        var result = await mediator.Send(updateCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(ApplicationErrors.UserEmailAlreadyExists(string.Empty).Code);
    }

    [Fact]
    public async Task Handle_WithDuplicatePhoneNumberForAnotherUser_ShouldReturnDuplicatePhoneNumberError()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create Admin A
        var emailA = $"admin_{Guid.NewGuid()}@example.com";
        var phoneA = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createA = new CreateAdminCommand("Admin A", emailA, "SecurePass123!", phoneA, nameof(UserRole.Admin));
        var resA = await mediator.Send(createA);
        resA.IsSuccess.Should().BeTrue();

        // 2. Create Admin B
        var emailB = $"admin_{Guid.NewGuid()}@example.com";
        var phoneB = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createB = new CreateAdminCommand("Admin B", emailB, "SecurePass123!", phoneB, nameof(UserRole.Admin));
        var resB = await mediator.Send(createB);
        var adminBId = resB.Value.Id;

        // 3. Try to update Admin B's phone number to match Admin A
        var updateCommand = new UpdateAdminCommand(
            Id: adminBId,
            Name: "Admin B Updated",
            Email: emailB,
            PhoneNumber: phoneA);

        // Act
        var result = await mediator.Send(updateCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(ApplicationErrors.UserPhoneNumberAlreadyExists(string.Empty).Code);
    }
}
