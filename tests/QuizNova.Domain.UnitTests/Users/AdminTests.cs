using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.UnitTests.Users;

public class AdminTests
{
    [Fact]
    public void Create_ShouldSuccess_WithValidData()
    {
        // Arrange
        var id = Guid.NewGuid();
        var personalInfoResult = PersonalInformation.Create(
            "Valid Admin Name",
            "admin@example.com",
            "1234567890");

        // Act
        var result = Admin.Create(id, personalInfoResult.Value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal("Valid Admin Name", result.Value.PersonalInformation.Name);
        Assert.Equal("admin@example.com", result.Value.PersonalInformation.Email);

        Assert.Empty(result.Value.DomainEvents);
    }

}
