using System.Net;

using FluentAssertions;

using QuizNova.Application.SubcutaneousTests.Common;

namespace QuizNova.Application.SubcutaneousTests.Features.Colleges.Queries.GetCollegeSummary;

public class GetCollegeSummaryQueryHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetCollegeSummary_AsInstructor_ShouldReturnForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync("ahmed.nasser@quiznova.local", "Instructor123!", "Instructor");

        // Act
        var response = await client.GetAsync("/colleges");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCollegeSummary_AsStudent_ShouldReturnForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync("omar.yasser@quiznova.local", "Student123!", "Student");

        // Act
        var response = await client.GetAsync("/colleges");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCollegeSummary_AsAdmin_ShouldReturnOk()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync("admin@quiznova.local", "Admin123!", "Admin");

        // Act
        var response = await client.GetAsync("/colleges");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCollegeSummary_Unauthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();

        // Act
        var response = await client.GetAsync("/colleges");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
