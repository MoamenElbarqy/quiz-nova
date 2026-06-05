using System.Net;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.IntegrationTests.Common;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Tests.Common.Security;

using Xunit;

namespace QuizNova.Api.IntegrationTests.Controllers;

public class CourseControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetCourses_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();

        // Act
        var response = await client.GetAsync("/courses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCourses_WhenStudent_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");

        // Act
        var response = await client.GetAsync("/courses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCourses_WhenInstructor_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");

        // Act
        var response = await client.GetAsync("/courses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCourses_WhenAdmin_ReturnsOkAndCourses()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Act
        var response = await client.GetAsync("/courses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedList<CourseDto>>();
        result.Should().NotBeNull();
        result.Items.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-5, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -10)]
    [InlineData(1, 101)]
    public async Task GetCourses_WithInvalidQuery_ReturnsBadRequest(int pageNumber, int pageSize)
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Act
        var response = await client.GetAsync($"/courses?PageNumber={pageNumber}&PageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCoursesCount_WithInstructorId_ReturnsCoursesCountDto()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var (_, instructorId, _) = await GetSeededIdsAsync();

        // Act
        var response = await client.GetAsync($"/instructor/{instructorId}/courses/count");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var countDto = await response.Content.ReadFromJsonAsync<CoursesCountDto>();
        countDto.Should().NotBeNull();
        countDto.CoursesCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetCourseById_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        var (courseId, _, _) = await GetSeededIdsAsync();

        // Act
        var response = await client.GetAsync($"/courses/{courseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCourseById_WithValidId_ReturnsCourseDto()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var (courseId, _, _) = await GetSeededIdsAsync();

        // Act
        var response = await client.GetAsync($"/courses/{courseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var course = await response.Content.ReadFromJsonAsync<CourseDto>();
        course.Should().NotBeNull();
        course.Id.Should().Be(courseId);
    }

    [Fact]
    public async Task GetCourseById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/courses/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCourseById_WithEmptyId_ReturnsBadRequest()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");

        // Act
        var response = await client.GetAsync($"/courses/{Guid.Empty}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCourse_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        var request = new CreateCourseRequest("New Test Course", null, 50, 100);

        // Act
        var response = await client.PostAsJsonAsync("/courses", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateCourse_WhenStudent_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var request = new CreateCourseRequest("New Test Course", null, 50, 100);

        // Act
        var response = await client.PostAsJsonAsync("/courses", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateCourse_WhenAdmin_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var request = new CreateCourseRequest("New Test Course", null, 50, 100);

        // Act
        var response = await client.PostAsJsonAsync("/courses", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateCourse_WhenInstructor_ReturnsCreatedCourse()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var (_, instructorId, _) = await GetSeededIdsAsync();

        var request = new CreateCourseRequest("New Integration Course", instructorId, 60, 100);

        // Act
        var response = await client.PostAsJsonAsync("/courses", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var course = await response.Content.ReadFromJsonAsync<CourseDto>();
        course.Should().NotBeNull();
        course.CourseName.Should().Be(request.Name);
        course.InstructorId.Should().Be(instructorId);
    }

    [Fact]
    public async Task UpdateCourseInstructor_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        var (courseId, instructorId, _) = await GetSeededIdsAsync();
        var request = new UpdateCourseInstructorRequest(instructorId);

        // Act
        var response = await client.PatchAsJsonAsync($"/courses/{courseId}/instructor", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateCourseInstructor_WhenStudent_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var (courseId, instructorId, _) = await GetSeededIdsAsync();
        var request = new UpdateCourseInstructorRequest(instructorId);

        // Act
        var response = await client.PatchAsJsonAsync($"/courses/{courseId}/instructor", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateCourseInstructor_WhenInstructor_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var (courseId, instructorId, _) = await GetSeededIdsAsync();
        var request = new UpdateCourseInstructorRequest(instructorId);

        // Act
        var response = await client.PatchAsJsonAsync($"/courses/{courseId}/instructor", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateCourseInstructor_WhenAdmin_ReturnsUpdatedCourse()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var (courseId, instructorId, _) = await GetSeededIdsAsync();
        var request = new UpdateCourseInstructorRequest(instructorId);

        // Act
        var response = await client.PatchAsJsonAsync($"/courses/{courseId}/instructor", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var course = await response.Content.ReadFromJsonAsync<CourseDto>();
        course.Should().NotBeNull();
        course.Id.Should().Be(courseId);
        course.InstructorId.Should().Be(instructorId);
    }

    [Fact]
    public async Task GetInstructorCourses_WithValidId_ReturnsCourseDtos()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var (_, instructorId, _) = await GetSeededIdsAsync();

        // Act
        var response = await client.GetAsync($"/instructor/{instructorId}/courses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var courses = await response.Content.ReadFromJsonAsync<List<CourseDto>>();
        courses.Should().NotBeNull();
        courses.Count.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetInstructorCourses_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/instructor/{nonExistentId}/courses");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCourseById_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        var (courseId, _, _) = await GetSeededIdsAsync();

        // Act
        var response = await client.DeleteAsync($"/courses/{courseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteCourseById_WhenStudent_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");
        var (courseId, _, _) = await GetSeededIdsAsync();

        // Act
        var response = await client.DeleteAsync($"/courses/{courseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteCourseById_WhenInstructor_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");
        var (courseId, _, _) = await GetSeededIdsAsync();

        // Act
        var response = await client.DeleteAsync($"/courses/{courseId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteCourseById_WhenAdmin_ReturnsNoContent()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Fetch courses first via GET /courses as requested, then try to delete one of them
        var getResponse = await client.GetAsync("/courses");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var paginatedCourses = await getResponse.Content.ReadFromJsonAsync<PaginatedList<CourseDto>>();
        paginatedCourses.Should().NotBeNull();
        paginatedCourses.Items.Should().NotBeEmpty();

        var courseToDelete = paginatedCourses.Items.First();

        // Act
        var response = await client.DeleteAsync($"/courses/{courseToDelete.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it is indeed deleted
        var getByIdResponse = await client.GetAsync($"/courses/{courseToDelete.Id}");
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<(Guid courseId, Guid instructorId, Guid studentId)> GetSeededIdsAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var course = await dbContext.Courses.FirstOrDefaultAsync()
                     ?? throw new InvalidOperationException("No courses found in database.");
        var instructor = await dbContext.Instructors.FirstOrDefaultAsync()
                         ?? throw new InvalidOperationException("No instructors found in database.");
        var student = await dbContext.Students.FirstOrDefaultAsync()
                      ?? throw new InvalidOperationException("No students found in database.");

        return (course.Id, instructor.Id, student.Id);
    }
}
