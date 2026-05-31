using System.Net;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.IntegrationTests.Common;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Tests.Common.Security;

using Xunit;

namespace QuizNova.Api.IntegrationTests.Controllers;

public class StudentControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetStudents_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();

        // Act
        var response = await client.GetAsync("/students");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetStudents_WhenStudent_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");

        // Act
        var response = await client.GetAsync("/students");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetStudents_WhenInstructor_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor.User.Email!, TestUsers.Instructor.Password, "Instructor");

        // Act
        var response = await client.GetAsync("/students");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetStudents_WhenAdmin_ReturnsOkAndStudents()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Act
        var response = await client.GetAsync("/students");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedList<StudentDto>>();
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-5, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -10)]
    [InlineData(1, 101)]
    public async Task GetStudents_WithInvalidQuery_ReturnsBadRequest(int pageNumber, int pageSize)
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Act
        var response = await client.GetAsync($"/students?PageNumber={pageNumber}&PageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetStudentById_WithValidId_ReturnsStudentDto()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var (studentId, _) = await GetSeededIdsAsync();

        // Act
        var response = await client.GetAsync($"/students/{studentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<StudentDto>();
        result.Should().NotBeNull();
        result.Id.Should().Be(studentId);
    }

    [Fact]
    public async Task GetStudentById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/students/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetStudentById_WithEmptyId_ReturnsBadRequest()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Act
        var response = await client.GetAsync($"/students/{Guid.Empty}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateStudent_WhenAdmin_ReturnsCreatedStudent()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var request = new CreateStudentRequest("Unique Stud", "uniquestud@quiznova.local", "StudPass123!", "01199999998", "Student");

        // Act
        var response = await client.PostAsJsonAsync("/students", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<StudentDto>();
        result.Should().NotBeNull();
        result.Email.Should().Be(request.Email);
        result.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task CreateStudent_WithDuplicateEmail_ReturnsConflict()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var request = new CreateStudentRequest("Duplicate Email Stud", TestUsers.Student.User.Email!, "StudPass123!", "01199999999", "Student");

        // Act
        var response = await client.PostAsJsonAsync("/students", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateStudent_CreateSameTwice_ReturnsConflict()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var request = new CreateStudentRequest("Twice Stud", "twicestud@quiznova.local", "StudPass123!", "01188888888", "Student");

        // Act - First call
        var response1 = await client.PostAsJsonAsync("/students", request);
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act - Second call
        var response2 = await client.PostAsJsonAsync("/students", request);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateStudent_WhenAdmin_ReturnsUpdatedStudent()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var (studentId, _) = await GetSeededIdsAsync();
        var request = new UpdateStudentRequest("Updated Student Name", "updatedstud@quiznova.local", "01166666666");

        // Act
        var response = await client.PutAsJsonAsync($"/students/{studentId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<StudentDto>();
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task UpdateStudent_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateStudentRequest("Nonexistent Name", "nonexistent@stud.local", "01155555555");

        // Act
        var response = await client.PutAsJsonAsync($"/students/{nonExistentId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteStudent_WhenAdmin_ReturnsNoContent()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Create a temp student so we don't break database state for other tests
        var request = new CreateStudentRequest("Temp Delete Stud", "tempdeletestud@quiznova.local", "StudPass123!", "01144444444", "Student");
        var createResponse = await client.PostAsJsonAsync("/students", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await createResponse.Content.ReadFromJsonAsync<StudentDto>();

        // Act
        var response = await client.DeleteAsync($"/students/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify indeed deleted
        var getResponse = await client.GetAsync($"/students/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteStudent_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/students/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteStudent_DeleteTwice_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        var request = new CreateStudentRequest("Temp Delete Stud Twice", "tempdeletestudtwice@quiznova.local", "StudPass123!", "01133333333", "Student");
        var createResponse = await client.PostAsJsonAsync("/students", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var created = await createResponse.Content.ReadFromJsonAsync<StudentDto>();

        // Act - First delete
        var response1 = await client.DeleteAsync($"/students/{created!.Id}");
        response1.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act - Second delete
        var response2 = await client.DeleteAsync($"/students/{created.Id}");

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<(Guid studentId, Guid instructorId)> GetSeededIdsAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var student = await dbContext.Students.FirstOrDefaultAsync()
                      ?? throw new InvalidOperationException("No students found in database.");
        var instructor = await dbContext.Instructors.FirstOrDefaultAsync()
                          ?? throw new InvalidOperationException("No instructors found in database.");

        return (student.Id, instructor.Id);
    }
}
