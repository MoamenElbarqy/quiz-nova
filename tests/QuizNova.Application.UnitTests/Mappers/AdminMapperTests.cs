using QuizNova.Application.Features.Admins.Mappers;
using QuizNova.Tests.Common.Users.Admins;

using Xunit;

namespace QuizNova.Application.UnitTests.Mappers;

public class AdminMapperTests
{
    [Fact]
    public void ToAdminDto_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var admin = AdminFactory.Create().Value;

        // Act
        var dto = admin.ToAdminDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(admin.Id, dto.AdminId);
        Assert.Equal(admin.PersonalInformation.Name, dto.Name);
        Assert.Equal(admin.PersonalInformation.Email, dto.Email);
        Assert.Equal(admin.PersonalInformation.PhoneNumber, dto.PhoneNumber);
    }
}
