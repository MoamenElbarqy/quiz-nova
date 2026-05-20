using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.Admins.Events;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;
using QuizNova.Tests.Common.Users.Admins;
using QuizNova.Tests.Common.Users.UserPersonalInformation;

namespace QuizNova.Domain.UnitTests.Users;

public class AdminTests
{
    [Fact]
    public void Create_ShouldSuccess_WithValidData()
    {
        // Arrange
        var personalInfoResult = PersonalInformation.Create(
            "Valid Admin Name",
            "admin@example.com",
            "SecurePassword123!",
            "1234567890");

        // Act
        var result = Admin.Create(personalInfoResult.Value, []);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal("Valid Admin Name", result.Value.PersonalInformation.Name);
        Assert.Equal("admin@example.com", result.Value.PersonalInformation.Email);

        var createdEvent = Assert.Single(result.Value.DomainEvents);
        var adminCreatedEvent = Assert.IsType<AdminCreatedEvent>(createdEvent);
        Assert.Equal(result.Value.Id, adminCreatedEvent.Id);
}

    [Fact]
    public void Update_ShouldSuccess_WithValidData()
    {
        // Arrange
        var admin = AdminFactory.CreateAdmin().Value;
        var newPersonalInfo = PersonalInformationFactory.CreatePersonalInformation(
            name: "Updated Admin Name",
            email: "updated.admin@example.com");
        admin.ClearDomainEvents();

        // Act
        var result = admin.Update(newPersonalInfo);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Admin Name", admin.PersonalInformation.Name);
        Assert.Equal("updated.admin@example.com", admin.PersonalInformation.Email);

        var updatedEvent = Assert.Single(admin.DomainEvents);
        var adminUpdatedEvent = Assert.IsType<AdminUpdatedEvent>(updatedEvent);
        Assert.Equal(admin.Id, adminUpdatedEvent.Id);
}

    [Fact]
    public void Delete_ShouldSuccess()
    {
        // Arrange
        var admin = AdminFactory.CreateAdmin().Value;
        admin.ClearDomainEvents();

        // Act
        var result = admin.Delete();

        // Assert
        Assert.True(result.IsSuccess);

        var deletedEvent = Assert.Single(admin.DomainEvents);
        var adminDeletedEvent = Assert.IsType<AdminDeletedEvent>(deletedEvent);
        Assert.Equal(admin.Id, adminDeletedEvent.Id);
}
}
