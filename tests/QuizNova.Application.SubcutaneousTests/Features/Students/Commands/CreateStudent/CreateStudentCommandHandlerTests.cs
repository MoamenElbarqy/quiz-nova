using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Students.Commands.CreateStudent;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.SubcutaneousTests.Features.Students.Commands.CreateStudent;

public class CreateStudentCommandHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithValidData_ShouldSuccess()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var uniqueEmail = $"student_{Guid.NewGuid()}@example.com";
        var uniquePhone = $"+1{Guid.NewGuid().ToString()[..10]}"; // ensure valid length between 7 and 15

        var command = new CreateStudentCommand(
            Name: "Valid Student Name",
            Email: uniqueEmail,
            Password: "SecurePass123!",
            PhoneNumber: uniquePhone,
            Role: nameof(UserRole.Student));

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue($"because creation should succeed but failed with: {result.TopError.Description}");
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be(uniqueEmail);

        // Verify existence in database
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
        var studentInDb = await dbContext.Students
            .FirstOrDefaultAsync(s => s.PersonalInformation.Email == uniqueEmail);

        studentInDb.Should().NotBeNull();
        studentInDb.PersonalInformation.Name.Should().Be("Valid Student Name");
        studentInDb.PersonalInformation.PhoneNumber.Should().Be(uniquePhone);
        studentInDb.UserRole.Should().Be(UserRole.Student);
    }

    [Fact]
    public async Task Handle_WithNameLessThanThreeChars_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new CreateStudentCommand(
            Name: "Ab",
            Email: "student@example.com",
            Password: "SecurePass123!",
            PhoneNumber: "+123456789",
            Role: nameof(UserRole.Student));

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Name" && e.Description.Contains("at least 3 characters"));
    }

    [Fact]
    public async Task Handle_WithEmptyEmail_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new CreateStudentCommand(
            Name: "Valid Name",
            Email: string.Empty,
            Password: "SecurePass123!",
            PhoneNumber: "+123456789",
            Role: nameof(UserRole.Student));

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Email" && e.Description.Contains("required"));
    }

    [Fact]
    public async Task Handle_WithInvalidEmailFormat_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new CreateStudentCommand(
            Name: "Valid Name",
            Email: "invalid-email",
            Password: "SecurePass123!",
            PhoneNumber: "+123456789",
            Role: nameof(UserRole.Student));

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Email" && e.Description.Contains("valid email address"));
    }

    [Fact]
    public async Task Handle_WithWeakPassword_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new CreateStudentCommand(
            Name: "Valid Name",
            Email: "student@example.com",
            Password: "weak",
            PhoneNumber: "+123456789",
            Role: nameof(UserRole.Student));

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Password");
    }

    [Fact]
    public async Task Handle_WithInvalidRoleForStudent_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new CreateStudentCommand(
            Name: "Valid Name",
            Email: "student@example.com",
            Password: "SecurePass123!",
            PhoneNumber: "+123456789",
            Role: nameof(UserRole.Instructor));

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "Role" && e.Description.Contains("Role must be 'Student'"));
    }

    [Fact]
    public async Task Handle_WithPhoneNumberLessThanSevenChars_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new CreateStudentCommand(
            Name: "Valid Name",
            Email: "student@example.com",
            Password: "SecurePass123!",
            PhoneNumber: "12345",
            Role: nameof(UserRole.Student));

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "PhoneNumber" && e.Description.Contains("between 7 and 15 characters"));
    }

    [Fact]
    public async Task Handle_WithPhoneNumberGreaterThanFifteenChars_ShouldReturnValidationError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var command = new CreateStudentCommand(
            Name: "Valid Name",
            Email: "student@example.com",
            Password: "SecurePass123!",
            PhoneNumber: "1234567890123456",
            Role: nameof(UserRole.Student));

        // Act
        var result = await mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "PhoneNumber" && e.Description.Contains("between 7 and 15 characters"));
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ShouldReturnDuplicateEmailError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var email = $"student_{Guid.NewGuid()}@example.com";
        var phone1 = $"+1{Guid.NewGuid().ToString()[..10]}";
        var phone2 = $"+1{Guid.NewGuid().ToString()[..10]}";

        var command1 = new CreateStudentCommand("Student One", email, "SecurePass123!", phone1, nameof(UserRole.Student));
        var command2 = new CreateStudentCommand("Student Two", email, "SecurePass123!", phone2, nameof(UserRole.Student));

        // Act
        var result1 = await mediator.Send(command1);
        var result2 = await mediator.Send(command2);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsError.Should().BeTrue();
        result2.TopError.Code.Should().Be("User.Email.AlreadyExists");
    }

    [Fact]
    public async Task Handle_WithDuplicatePhoneNumber_ShouldReturnDuplicatePhoneNumberError()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var email1 = $"student_{Guid.NewGuid()}@example.com";
        var email2 = $"student_{Guid.NewGuid()}@example.com";
        var phone = $"+1{Guid.NewGuid().ToString()[..10]}";

        var command1 = new CreateStudentCommand("Student One", email1, "SecurePass123!", phone, nameof(UserRole.Student));
        var command2 = new CreateStudentCommand("Student Two", email2, "SecurePass123!", phone, nameof(UserRole.Student));

        // Act
        var result1 = await mediator.Send(command1);
        var result2 = await mediator.Send(command2);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsError.Should().BeTrue();
        result2.TopError.Code.Should().Be("User.PhoneNumber.AlreadyExists");
    }
}
