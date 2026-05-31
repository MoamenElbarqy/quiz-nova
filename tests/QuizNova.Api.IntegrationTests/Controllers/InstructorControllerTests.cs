using System.Net;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.IntegrationTests.Common;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Instructors.DTOs;
using QuizNova.Tests.Common.Security;

using Xunit;

namespace QuizNova.Api.IntegrationTests.Controllers;

public class InstructorControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetInstructors_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();

        // Act
        var response = await client.GetAsync("/instructors");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetInstructors_WhenStudent_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");

        // Act
        var response = await client.GetAsync("/instructors");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetInstructors_WhenInstructor_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor.User.Email!, TestUsers.Instructor.Password, "Instructor");

        // Act
        var response = await client.GetAsync("/instructors");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetInstructors_WhenAdmin_ReturnsOkAndInstructors()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Act
        var response = await client.GetAsync("/instructors?PageNumber=1&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedList<InstructorDto>>();
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-5, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -10)]
    [InlineData(1, 101)]
    public async Task GetInstructors_WithInvalidQuery_ReturnsBadRequest(int pageNumber, int pageSize)
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Act
        var response = await client.GetAsync($"/instructors?PageNumber={pageNumber}&PageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetInstructorById_WithValidId_ReturnsInstructorDto()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var (_, instructorId) = await GetSeededIdsAsync();

        // Act
        var response = await client.GetAsync($"/instructors/{instructorId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<InstructorDto>();
        result.Should().NotBeNull();
        result.Id.Should().Be(instructorId);
    }

    [Fact]
    public async Task GetInstructorById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/instructors/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetInstructorById_WithEmptyId_ReturnsBadRequest()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Act
        var response = await client.GetAsync($"/instructors/{Guid.Empty}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateInstructor_WhenAdmin_ReturnsCreatedInstructor()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var request = new CreateInstructorRequest("Unique Inst", "uniqueinst@quiznova.local", "InstPass123!",
            "01099999998", "Instructor");

        // Act
        var response = await client.PostAsJsonAsync("/instructors", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<InstructorDto>();
        result.Should().NotBeNull();
        result.Email.Should().Be(request.Email);
        result.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task CreateInstructor_WithDuplicateEmail_ReturnsConflict()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var request = new CreateInstructorRequest("Duplicate Email Inst", TestUsers.Instructor.User.Email!,
            "InstPass123!", "01099999999", "Instructor");

        // Act
        var response = await client.PostAsJsonAsync("/instructors", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateInstructor_CreateSameTwice_ReturnsConflict()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var request = new CreateInstructorRequest("Twice Inst", "twiceinst@quiznova.local", "InstPass123!",
            "01088888888", "Instructor");

        // Act - First call
        var response1 = await client.PostAsJsonAsync("/instructors", request);
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act - Second call
        var response2 = await client.PostAsJsonAsync("/instructors", request);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateInstructor_WhenAdmin_ReturnsUpdatedInstructor()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var (_, instructorId) = await GetSeededIdsAsync();
        var request =
            new UpdateInstructorRequest("Updated Instructor Name", "updatedinst@quiznova.local", "01066666666");

        // Act
        var response = await client.PutAsJsonAsync($"/instructors/{instructorId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<InstructorDto>();
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task UpdateInstructor_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateInstructorRequest("Nonexistent Name", "nonexistent@inst.local", "01055555555");

        // Act
        var response = await client.PutAsJsonAsync($"/instructors/{nonExistentId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteInstructor_WhenAdmin_ReturnsNoContent()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Create a temp instructor so we don't break database state for other tests
        var request = new CreateInstructorRequest("Temp Delete Inst", "tempdeleteinst@quiznova.local", "InstPass123!",
            "01044444444", "Instructor");
        var createResponse = await client.PostAsJsonAsync("/instructors", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await createResponse.Content.ReadFromJsonAsync<InstructorDto>();

        // Act
        var response = await client.DeleteAsync($"/instructors/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify indeed deleted
        var getResponse = await client.GetAsync($"/instructors/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteInstructor_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/instructors/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteInstructor_DeleteTwice_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        var request = new CreateInstructorRequest("Temp Delete Inst Twice", "tempdeletetwice@quiznova.local",
            "InstPass123!", "01033333333", "Instructor");
        var createResponse = await client.PostAsJsonAsync("/instructors", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await createResponse.Content.ReadFromJsonAsync<InstructorDto>();

        // Act - First delete
        var response1 = await client.DeleteAsync($"/instructors/{created!.Id}");
        response1.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act - Second delete
        var response2 = await client.DeleteAsync($"/instructors/{created.Id}");

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<(Guid studentId, Guid instructorId)> GetSeededIdsAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var student = await dbContext.Students.FirstOrDefaultAsync() ??
                      throw new InvalidOperationException("No students found in database.");
        var instructor = await dbContext.Instructors.FirstOrDefaultAsync() ??
                         throw new InvalidOperationException("No instructors found in database.");

        return (student.Id, instructor.Id);
    }
}
