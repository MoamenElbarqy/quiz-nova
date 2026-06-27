using QuizNova.Domain.Entities.Users.Student;
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
        var id = Guid.NewGuid();
        var personalInfoResult = PersonalInformation.Create(
            "Valid Student Name",
            "student@example.com",
            "1234567890");

        // Act
        var result = Student.Create(
            id,
            personalInfoResult.Value,
            [],
            []);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal("Valid Student Name", result.Value.PersonalInformation.Name);
        Assert.Equal("student@example.com", result.Value.PersonalInformation.Email);

        Assert.Empty(result.Value.DomainEvents);
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

        Assert.Empty(student.DomainEvents);
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

        Assert.Empty(student.DomainEvents);
    }
}
