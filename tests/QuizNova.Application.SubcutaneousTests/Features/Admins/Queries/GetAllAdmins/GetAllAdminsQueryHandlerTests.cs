using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Admins.Queries.GetAllAdmins;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Tests.Common.Users.Admins;
using QuizNova.Tests.Common.Users.UserPersonalInformation;

namespace QuizNova.Application.SubcutaneousTests.Features.Admins.Queries.GetAllAdmins;

public class GetAllAdminsQueryHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithDefaultPagination_ShouldReturnPaginatedList()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var query = new GetAllAdminsQuery();

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PageNumber.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_ShouldFilterCorrectly()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var uniqueSearchTerm = $"UniqueAdmin_{Guid.NewGuid()}";

        var admin1 = AdminFactory.Create(
            personalInformation: PersonalInformationFactory.CreatePersonalInformation(
                name: uniqueSearchTerm,
                email: $"admin_{Guid.NewGuid()}@example.com")).Value;

        var admin2 = AdminFactory.Create(
            personalInformation: PersonalInformationFactory.CreatePersonalInformation(
                name: "Another Admin Name",
                email: $"admin_{Guid.NewGuid()}@example.com")).Value;

        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
            await mongoContext.Users.InsertManyAsync([admin1, admin2]);

        }

        var query = new GetAllAdminsQuery(SearchTerm: uniqueSearchTerm);

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().ContainSingle(a => a.Id == admin1.Id);
        result.Value.Items.Should().NotContain(a => a.Id == admin2.Id);
    }
}
