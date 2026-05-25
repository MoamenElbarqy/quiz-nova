using System.Net;

using FluentAssertions;

using QuizNova.Api.IntegrationTests.Common;
using QuizNova.Application.Features.Colleges.DTOs;
using QuizNova.Tests.Common.Security;

using Xunit;

namespace QuizNova.Api.IntegrationTests.Controllers;

public class CollegeControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetSummary_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();

        // Act
        var response = await client.GetAsync("/colleges");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSummary_WhenStudent_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password);

        // Act
        var response = await client.GetAsync("/colleges");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSummary_WhenInstructor_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor.User.Email!, TestUsers.Instructor.Password);

        // Act
        var response = await client.GetAsync("/colleges");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSummary_WhenAdmin_ReturnsOkAndCollegeSummaryDto()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password);

        // Act
        var response = await client.GetAsync("/colleges");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var summary = await response.Content.ReadFromJsonAsync<CollegeSummaryDto>();
        summary.Should().NotBeNull();
    }

}
