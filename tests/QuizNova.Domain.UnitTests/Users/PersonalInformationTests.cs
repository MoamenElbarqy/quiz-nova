using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Domain.UnitTests.Users;

public class PersonalInformationTests
{
    [Fact]
    public void Create_ShouldSuccess_WithValidData()
    {
        // Act
        var result = PersonalInformation.Create(
            "Valid User Name",
            "user@example.com",
            "SecurePassword123!",
            "1234567890");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Valid User Name", result.Value.Name);
        Assert.Equal("user@example.com", result.Value.Email);
        Assert.Equal("SecurePassword123!", result.Value.Password);
        Assert.Equal("1234567890", result.Value.PhoneNumber);
}

    [Fact]
    public void Create_ShouldTrimInputData_WhenValidDataProvided()
    {
        // Act
        var result = PersonalInformation.Create(
            "   Valid User Name   ",
            "   user@example.com   ",
            "   SecurePassword123!   ",
            "   1234567890   ");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Valid User Name", result.Value.Name);
        Assert.Equal("user@example.com", result.Value.Email);
        Assert.Equal("SecurePassword123!", result.Value.Password);
        Assert.Equal("1234567890", result.Value.PhoneNumber);
}

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ShouldFail_WithEmptyNameOrNull(string? name)
    {
        // Act
        var result = PersonalInformation.Create(
            name!,
            "user@example.com",
            "SecurePassword123!",
            "1234567890");

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(PersonalInformationErrors.NameRequired, result.TopError);
}

    [Theory]
    [InlineData("a")]
    [InlineData("aa")]
    [InlineData("    aa           ")]
    [InlineData("        aa")]
    [InlineData("aa          ")]
    public void Create_ShouldFail_WithInvalidName(string name)
    {
        // Act
        var result = PersonalInformation.Create(
            name,
            "user@example.com",
            "SecurePassword123!",
            "1234567890");

        // Assert
        Assert.True(result.IsError);

        if (string.IsNullOrWhiteSpace(name))
        {
            Assert.Equal(PersonalInformationErrors.NameInvalid, result.TopError);
        }
}

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ShouldFail_WithEmptyEmailOrNull(string? email)
    {
        // Act
        var result = PersonalInformation.Create(
            "Valid User Name",
            email!,
            "SecurePassword123!",
            "1234567890");

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(PersonalInformationErrors.EmailRequired, result.TopError);
}

    [Theory]
    [InlineData("@gmail.com")]
    [InlineData("      ")]
    [InlineData("aa")]
    [InlineData("aaa")]
    [InlineData("a@.com")]
    [InlineData("a@gmail")]
    public void Create_ShouldFail_WithInvalidEmail(string email)
    {
        // Act
        var result = PersonalInformation.Create(
            "Valid User Name",
            email,
            "SecurePassword123!",
            "1234567890");

        // Assert
        Assert.True(result.IsError);

        if (string.IsNullOrWhiteSpace(email))
        {
            Assert.Equal(PersonalInformationErrors.EmailRequired, result.TopError);
        }
        else
        {
            Assert.Equal(PersonalInformationErrors.EmailInvalid, result.TopError);
        }
}

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ShouldFail_WithEmptyPasswordOrNull(string? password)
    {
        // Act
        var result = PersonalInformation.Create(
            "Valid User Name",
            "user@example.com",
            password!,
            "1234567890");

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(PersonalInformationErrors.PasswordRequired, result.TopError);
}

    [Theory]
    [InlineData("123")]
    [InlineData("1234567")]
    [InlineData("  1234567  ")]
    public void Create_ShouldFail_WithInvalidPassword(string password)
    {
        // Act
        var result = PersonalInformation.Create(
            "Valid User Name",
            "user@example.com",
            password,
            "1234567890");

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(PersonalInformationErrors.PasswordInvalid, result.TopError);
}

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ShouldFail_WithEmptyPhoneOrNull(string? phone)
    {
        // Act
        var result = PersonalInformation.Create(
            "Valid User Name",
            "user@example.com",
            "SecurePassword123!",
            phone!);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(PersonalInformationErrors.PhoneNumberRequired, result.TopError);
}

    [Theory]
    [InlineData("               a")]
    [InlineData("aaaa               ")]
    [InlineData("123")]
    [InlineData("12345678910111213")]
    public void Create_ShouldFail_WithInvalidPhone(string phone)
    {
        // Act
        var result = PersonalInformation.Create(
            "Valid User Name",
            "user@example.com",
            "SecurePassword123!",
            phone);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(PersonalInformationErrors.PhoneNumberInvalid, result.TopError);
}
}
