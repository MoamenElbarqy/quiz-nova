using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructors.Commands.CreateInstructor;
using QuizNova.Application.Features.Instructors.Commands.UpdateInstructor;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.SubcutaneousTests.Features.Instructors.Commands.UpdateInstructor;

public class UpdateInstructorCommandHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithEmptyId_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new UpdateInstructorCommand(
            Id: Guid.Empty,
            Name: "Valid Name",
            Email: "instructor@example.com",
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
        var command = new UpdateInstructorCommand(
            Id: nonExistentId,
            Name: "Valid Name",
            Email: "instructor@example.com",
            PhoneNumber: "+123456789");

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(ApplicationErrors.InstructorNotFound(Guid.Empty).Code);
    }

    [Fact]
    public async Task Handle_WithExistingId_ShouldUpdateSuccessfully()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Create a valid Instructor first
        var uniqueEmail1 = $"instructor_{Guid.NewGuid()}@example.com";
        var uniquePhone1 = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createCommand = new CreateInstructorCommand("Original Name", uniqueEmail1, "SecurePass123!", uniquePhone1, nameof(UserRole.Instructor));
        var createResult = await mediator.Send(createCommand);
        createResult.IsSuccess.Should().BeTrue();

        var instructorId = createResult.Value.Id;

        // 2. Prepare Update Command
        var uniqueEmail2 = $"instructor_{Guid.NewGuid()}@example.com";
        var uniquePhone2 = $"+1{Guid.NewGuid().ToString()[..10]}";
        var updateCommand = new UpdateInstructorCommand(
            Id: instructorId,
            Name: "Updated Instructor Name",
            Email: uniqueEmail2,
            PhoneNumber: uniquePhone2);

        // Act
        var updateResult = await mediator.Send(updateCommand);

        // Assert
        updateResult.IsSuccess.Should().BeTrue();
        updateResult.Value.Name.Should().Be("Updated Instructor Name");
        updateResult.Value.Email.Should().Be(uniqueEmail2);

        // Verify updated in database
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var instructorInDb = await dbContext.Instructors.FirstOrDefaultAsync(i => i.Id == instructorId);

        instructorInDb.Should().NotBeNull();
        instructorInDb.PersonalInformation.Name.Should().Be("Updated Instructor Name");
        instructorInDb.PersonalInformation.Email.Should().Be(uniqueEmail2);
        instructorInDb.PersonalInformation.PhoneNumber.Should().Be(uniquePhone2);
    }

    [Fact]
    public async Task Handle_WithInvalidData_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // Create Instructor
        var uniqueEmail = $"instructor_{Guid.NewGuid()}@example.com";
        var uniquePhone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createCommand = new CreateInstructorCommand("Original Name", uniqueEmail, "SecurePass123!", uniquePhone, nameof(UserRole.Instructor));
        var createResult = await mediator.Send(createCommand);
        var instructorId = createResult.Value.Id;

        // Update with name too short
        var updateCommand = new UpdateInstructorCommand(
            Id: instructorId,
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

        // 1. Create Instructor A
        var emailA = $"instructor_{Guid.NewGuid()}@example.com";
        var phoneA = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createA = new CreateInstructorCommand("Instructor A", emailA, "SecurePass123!", phoneA, nameof(UserRole.Instructor));
        var resA = await mediator.Send(createA);
        resA.IsSuccess.Should().BeTrue();

        // 2. Create Instructor B
        var emailB = $"instructor_{Guid.NewGuid()}@example.com";
        var phoneB = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createB = new CreateInstructorCommand("Instructor B", emailB, "SecurePass123!", phoneB, nameof(UserRole.Instructor));
        var resB = await mediator.Send(createB);
        var instructorBId = resB.Value.Id;

        // 3. Try to update Instructor B's email to match Instructor A
        var updateCommand = new UpdateInstructorCommand(
            Id: instructorBId,
            Name: "Instructor B Updated",
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

        // 1. Create Instructor A
        var emailA = $"instructor_{Guid.NewGuid()}@example.com";
        var phoneA = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createA = new CreateInstructorCommand("Instructor A", emailA, "SecurePass123!", phoneA, nameof(UserRole.Instructor));
        var resA = await mediator.Send(createA);
        resA.IsSuccess.Should().BeTrue();

        // 2. Create Instructor B
        var emailB = $"instructor_{Guid.NewGuid()}@example.com";
        var phoneB = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createB = new CreateInstructorCommand("Instructor B", emailB, "SecurePass123!", phoneB, nameof(UserRole.Instructor));
        var resB = await mediator.Send(createB);
        var instructorBId = resB.Value.Id;

        // 3. Try to update Instructor B's phone number to match Instructor A
        var updateCommand = new UpdateInstructorCommand(
            Id: instructorBId,
            Name: "Instructor B Updated",
            Email: emailB,
            PhoneNumber: phoneA);

        // Act
        var result = await mediator.Send(updateCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be(ApplicationErrors.UserPhoneNumberAlreadyExists(string.Empty).Code);
    }
}
