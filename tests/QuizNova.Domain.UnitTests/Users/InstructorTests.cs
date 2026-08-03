using QuizNova.Domain.Entities.Users.Instructors;
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
        var id = Guid.NewGuid();
        var personalInfoResult = PersonalInformation.Create(
            "Valid Instructor Name",
            "instructor@example.com",
            "1234567890");

        // Act
        var result = Instructor.Create(
            id,
            personalInfoResult.Value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal("Valid Instructor Name", result.Value.PersonalInformation.Name);
        Assert.Equal("instructor@example.com", result.Value.PersonalInformation.Email);

        Assert.Empty(result.Value.DomainEvents);
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

        Assert.Empty(instructor.DomainEvents);
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

        Assert.Empty(instructor.DomainEvents);
    }
}
