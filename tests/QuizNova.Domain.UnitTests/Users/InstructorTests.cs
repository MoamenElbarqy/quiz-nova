using QuizNova.Domain.Entities.Users.Instructors;
using QuizNova.Domain.Entities.Users.Instructors.Events;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;
using QuizNova.Tests.Common.Users.Instructors;
using QuizNova.Tests.Common.Users.UserPersonalInformation;

namespace QuizNova.Domain.UnitTests.Users;

public class InstructorTests
{
    [Fact]
    public void Create_ShouldSuccess_WithValidData()
    {
        // Arrange
        var personalInfoResult = PersonalInformation.Create(
            "Valid Instructor Name",
            "instructor@example.com",
            "SecurePassword123!",
            "1234567890");

        // Act
        var result = Instructor.Create(
            personalInfoResult.Value,
            [],
            [],
            []);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal("Valid Instructor Name", result.Value.PersonalInformation.Name);
        Assert.Equal("instructor@example.com", result.Value.PersonalInformation.Email);

        var createdEvent = Assert.Single(result.Value.DomainEvents);
        var instructorCreatedEvent = Assert.IsType<InstructorCreatedEvent>(createdEvent);
        Assert.Equal(result.Value.Id, instructorCreatedEvent.Id);
}

    [Fact]
    public void Update_ShouldSuccess_WithValidData()
    {
        // Arrange
        var instructor = InstructorFactory.CreateInstructor().Value;
        var newPersonalInfo = PersonalInformationFactory.CreatePersonalInformation(
            name: "Updated Instructor Name",
            email: "updated.instructor@example.com");
        instructor.ClearDomainEvents();

        // Act
        var result = instructor.Update(newPersonalInfo);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Instructor Name", instructor.PersonalInformation.Name);
        Assert.Equal("updated.instructor@example.com", instructor.PersonalInformation.Email);

        var updatedEvent = Assert.Single(instructor.DomainEvents);
        var instructorUpdatedEvent = Assert.IsType<InstructorUpdatedEvent>(updatedEvent);
        Assert.Equal(instructor.Id, instructorUpdatedEvent.Id);
}

    [Fact]
    public void Delete_ShouldSuccess()
    {
        // Arrange
        var instructor = InstructorFactory.CreateInstructor().Value;
        instructor.ClearDomainEvents();

        // Act
        var result = instructor.Delete();

        // Assert
        Assert.True(result.IsSuccess);

        var deletedEvent = Assert.Single(instructor.DomainEvents);
        var instructorDeletedEvent = Assert.IsType<InstructorDeletedEvent>(deletedEvent);
        Assert.Equal(instructor.Id, instructorDeletedEvent.Id);
}
}
