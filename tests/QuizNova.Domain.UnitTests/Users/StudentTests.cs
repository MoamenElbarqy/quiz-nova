using QuizNova.Domain.Entities.Users.Student;
using QuizNova.Domain.Entities.Users.Student.Events;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;
using QuizNova.Tests.Common.Users.Students;
using QuizNova.Tests.Common.Users.UserPersonalInformation;

namespace QuizNova.Domain.UnitTests.Users;

public class StudentTests
{
    [Fact]
    public void Create_ShouldSuccess_WithValidData()
    {
        // Arrange
        var personalInfoResult = PersonalInformation.Create(
            "Valid Student Name",
            "student@example.com",
            "SecurePassword123!",
            "1234567890");

        // Act
        var result = Student.Create(
            personalInfoResult.Value,
            [],
            [],
            []);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal("Valid Student Name", result.Value.PersonalInformation.Name);
        Assert.Equal("student@example.com", result.Value.PersonalInformation.Email);

        var createdEvent = Assert.Single(result.Value.DomainEvents);
        var studentCreatedEvent = Assert.IsType<StudentCreatedEvent>(createdEvent);
        Assert.Equal(result.Value.Id, studentCreatedEvent.Id);
}

    [Fact]
    public void Update_ShouldSuccess_WithValidData()
    {
        // Arrange
        var student = StudentFactory.CreateStudent().Value;
        var newPersonalInfo = PersonalInformationFactory.CreatePersonalInformation(
            name: "Updated Student Name",
            email: "updated.student@example.com");
        student.ClearDomainEvents();

        // Act
        var result = student.Update(newPersonalInfo);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Student Name", student.PersonalInformation.Name);
        Assert.Equal("updated.student@example.com", student.PersonalInformation.Email);

        var updatedEvent = Assert.Single(student.DomainEvents);
        var studentUpdatedEvent = Assert.IsType<StudentUpdatedEvent>(updatedEvent);
        Assert.Equal(student.Id, studentUpdatedEvent.Id);
}

    [Fact]
    public void Delete_ShouldSuccess()
    {
        // Arrange
        var student = StudentFactory.CreateStudent().Value;
        student.ClearDomainEvents();

        // Act
        var result = student.Delete();

        // Assert
        Assert.True(result.IsSuccess);

        var deletedEvent = Assert.Single(student.DomainEvents);
        var studentDeletedEvent = Assert.IsType<StudentDeletedEvent>(deletedEvent);
        Assert.Equal(student.Id, studentDeletedEvent.Id);
}
}
