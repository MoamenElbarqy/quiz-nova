using System.Net;

using FluentAssertions;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.IntegrationTests.Common;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Tests.Common.Security;

using Xunit;

namespace QuizNova.Api.IntegrationTests.Controllers;

public class AdminControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Get_WhenUnauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();

        // Act
        var response = await client.GetAsync("/admins");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_WhenStudent_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Student.User.Email!, TestUsers.Student.Password, "Student");

        // Act
        var response = await client.GetAsync("/admins");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_WhenInstructor_ReturnsForbidden()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Instructor1.User.Email!, TestUsers.Instructor1.Password, "Instructor");

        // Act
        var response = await client.GetAsync("/admins");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllAdmins_WithValidAdmin_ReturnsAdmins()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Act
        var response = await client.GetAsync("/admins");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var paginatedAdmins = await response.Content.ReadFromJsonAsync<PaginatedList<AdminDto>>();
        paginatedAdmins.Should().NotBeNull();
        paginatedAdmins.Items.Should().NotBeEmpty();
        paginatedAdmins.Items.Should().Contain(a => a.PersonalInformation.Email == TestUsers.Admin.User.Email);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-5, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -10)]
    [InlineData(1, 101)]
    public async Task GetAllAdmins_WithInvalidQuery_ReturnsBadRequest(int pageNumber, int pageSize)
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // Act
        var response = await client.GetAsync($"/admins?PageNumber={pageNumber}&PageSize={pageSize}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAdminById_WithValidId_ReturnsAdmin()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        var getAdminsResponse = await client.GetAsync("/admins");
        getAdminsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var paginatedAdmins = await getAdminsResponse.Content.ReadFromJsonAsync<PaginatedList<AdminDto>>();
        paginatedAdmins.Should().NotBeNull();
        var adminId = paginatedAdmins.Items.First(a => a.PersonalInformation.Email == TestUsers.Admin.User.Email).Id;

        // Act
        var response = await client.GetAsync($"/admins/{adminId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var admin = await response.Content.ReadFromJsonAsync<AdminDto>();
        admin.Should().NotBeNull();
        admin.Id.Should().Be(adminId);
        admin.PersonalInformation.Email.Should().Be(TestUsers.Admin.User.Email);
    }

    [Fact]
    public async Task GetAdminById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/admins/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAdminById_WithEmptyId_ReturnsBadRequest()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var emptyId = Guid.Empty;

        // Act
        var response = await client.GetAsync($"/admins/{emptyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAdmin_WithValidPayload_ReturnsCreatedAdmin()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        var request = new CreateAdminRequest(
            "New Admin",
            "newadmin@quiznova.local",
            "AdminPassword123!",
            "+12345678901",
            "Admin");

        // Act
        var response = await client.PostAsJsonAsync("/admins", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var admin = await response.Content.ReadFromJsonAsync<AdminDto>();
        admin.Should().NotBeNull();
        admin!.PersonalInformation.Email.Should().Be(request.Email);
        admin.PersonalInformation.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task CreateAdmin_WithDuplicateEmail_ReturnsConflict()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var request = new CreateAdminRequest(
            "Another Admin",
            TestUsers.Admin.User.Email!,
            "Password123!",
            "+9999999999",
            "Admin");

        // Act
        var response = await client.PostAsJsonAsync("/admins", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateAdmin_WithValidPayload_ReturnsUpdatedAdmin()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        var getAdminsResponse = await client.GetAsync("/admins");
        getAdminsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var paginatedAdmins = await getAdminsResponse.Content.ReadFromJsonAsync<PaginatedList<AdminDto>>();
        paginatedAdmins.Should().NotBeNull();
        var adminId = paginatedAdmins.Items.First(a => a.PersonalInformation.Email == TestUsers.Admin.User.Email).Id;

        var request = new UpdateAdminRequest(
            "Updated Admin Name",
            TestUsers.Admin.User.Email!,
            "+1112223334");

        // Act
        var response = await client.PutAsJsonAsync($"/admins/{adminId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var admin = await response.Content.ReadFromJsonAsync<AdminDto>();
        admin.Should().NotBeNull();
        admin!.PersonalInformation.Name.Should().Be(request.Name);
        admin.PersonalInformation.PhoneNumber.Should().Be(request.PhoneNumber);
    }

    [Fact]
    public async Task UpdateAdmin_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateAdminRequest(
            "NonExistent Admin",
            "nonexistentadmin@quiznova.local",
            "+12345678");

        // Act
        var response = await client.PutAsJsonAsync($"/admins/{nonExistentId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAdmin_WithValidId_ReturnsNoContent()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");

        // 1. Create a temporary admin to delete
        var createRequest = new CreateAdminRequest(
            "Temp Admin",
            "tempadmin@quiznova.local",
            "TempPassword123!",
            "+77777777777",
            "Admin");
        var createResponse = await client.PostAsJsonAsync("/admins", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdAdmin = await createResponse.Content.ReadFromJsonAsync<AdminDto>();
        var tempAdminId = createdAdmin!.Id;

        // Act
        var deleteResponse = await client.DeleteAsync($"/admins/{tempAdminId}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteAdmin_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        using var client = factory.CreateAppHttpClient();
        await client.AuthenticateAsync(TestUsers.Admin.User.Email!, TestUsers.Admin.Password, "Admin");
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/admins/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
