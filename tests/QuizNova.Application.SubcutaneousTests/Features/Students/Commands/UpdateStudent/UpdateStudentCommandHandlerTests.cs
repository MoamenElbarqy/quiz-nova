using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Students.Commands.CreateStudent;
using QuizNova.Application.Features.Students.Commands.UpdateStudent;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.SubcutaneousTests.Features.Students.Commands.UpdateStudent;

public class UpdateStudentCommandHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithEmptyId_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new UpdateStudentCommand(
            Id: Guid.Empty,
            Name: "Valid Name",
            Email: "student@example.com",
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
        var command = new UpdateStudentCommand(
            Id: nonExistentId,
            Name: "Valid Name",
            Email: "student@example.com",
            PhoneNumber: "+123456789");

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("Student.NotFound");
    }

    [Fact]
    public async Task Handle_WithExistingId_ShouldUpdateSuccessfully()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        
        // 1. Create a valid Student first
        var uniqueEmail1 = $"student_{Guid.NewGuid()}@example.com";
        var uniquePhone1 = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createCommand = new CreateStudentCommand("Original Name", uniqueEmail1, "SecurePass123!", uniquePhone1, nameof(UserRole.Student));
        var createResult = await mediator.Send(createCommand);
        createResult.IsSuccess.Should().BeTrue();
        
        var studentId = createResult.Value.Id;

        // 2. Prepare Update Command
        var uniqueEmail2 = $"student_{Guid.NewGuid()}@example.com";
        var uniquePhone2 = $"+1{Guid.NewGuid().ToString()[..10]}";
        var updateCommand = new UpdateStudentCommand(
            Id: studentId,
            Name: "Updated Student Name",
            Email: uniqueEmail2,
            PhoneNumber: uniquePhone2);

        // Act
        var updateResult = await mediator.Send(updateCommand);

        // Assert
        updateResult.IsSuccess.Should().BeTrue();
        updateResult.Value.Name.Should().Be("Updated Student Name");
        updateResult.Value.Email.Should().Be(uniqueEmail2);

        // Verify updated in database
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var studentInDb = await dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId);
        
        studentInDb.Should().NotBeNull();
        studentInDb.PersonalInformation.Name.Should().Be("Updated Student Name");
        studentInDb.PersonalInformation.Email.Should().Be(uniqueEmail2);
        studentInDb.PersonalInformation.PhoneNumber.Should().Be(uniquePhone2);
    }

    [Fact]
    public async Task Handle_WithInvalidData_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        
        // Create Student
        var uniqueEmail = $"student_{Guid.NewGuid()}@example.com";
        var uniquePhone = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createCommand = new CreateStudentCommand("Original Name", uniqueEmail, "SecurePass123!", uniquePhone, nameof(UserRole.Student));
        var createResult = await mediator.Send(createCommand);
        var studentId = createResult.Value.Id;

        // Update with name too short
        var updateCommand = new UpdateStudentCommand(
            Id: studentId,
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
        
        // 1. Create Student A
        var emailA = $"student_{Guid.NewGuid()}@example.com";
        var phoneA = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createA = new CreateStudentCommand("Student A", emailA, "SecurePass123!", phoneA, nameof(UserRole.Student));
        var resA = await mediator.Send(createA);
        resA.IsSuccess.Should().BeTrue();

        // 2. Create Student B
        var emailB = $"student_{Guid.NewGuid()}@example.com";
        var phoneB = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createB = new CreateStudentCommand("Student B", emailB, "SecurePass123!", phoneB, nameof(UserRole.Student));
        var resB = await mediator.Send(createB);
        var studentBId = resB.Value.Id;

        // 3. Try to update Student B's email to match Student A
        var updateCommand = new UpdateStudentCommand(
            Id: studentBId,
            Name: "Student B Updated",
            Email: emailA,
            PhoneNumber: phoneB);

        // Act
        var result = await mediator.Send(updateCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("User.Email.AlreadyExists");
    }

    [Fact]
    public async Task Handle_WithDuplicatePhoneNumberForAnotherUser_ShouldReturnDuplicatePhoneNumberError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        
        // 1. Create Student A
        var emailA = $"student_{Guid.NewGuid()}@example.com";
        var phoneA = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createA = new CreateStudentCommand("Student A", emailA, "SecurePass123!", phoneA, nameof(UserRole.Student));
        var resA = await mediator.Send(createA);
        resA.IsSuccess.Should().BeTrue();

        // 2. Create Student B
        var emailB = $"student_{Guid.NewGuid()}@example.com";
        var phoneB = $"+1{Guid.NewGuid().ToString()[..10]}";
        var createB = new CreateStudentCommand("Student B", emailB, "SecurePass123!", phoneB, nameof(UserRole.Student));
        var resB = await mediator.Send(createB);
        var studentBId = resB.Value.Id;

        // 3. Try to update Student B's phone number to match Student A
        var updateCommand = new UpdateStudentCommand(
            Id: studentBId,
            Name: "Student B Updated",
            Email: emailB,
            PhoneNumber: phoneA);

        // Act
        var result = await mediator.Send(updateCommand);

        // Assert
        result.IsError.Should().BeTrue();
        result.TopError.Code.Should().Be("User.PhoneNumber.AlreadyExists");
    }
}
