using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructors.Queries.GetAllInstructors;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Tests.Common.Courses;
using QuizNova.Tests.Common.Quizzes;
using QuizNova.Tests.Common.Users.Instructors;
using QuizNova.Tests.Common.Users.UserPersonalInformation;

namespace QuizNova.Application.SubcutaneousTests.Features.Instructors.Queries.GetAllInstructors;

public class GetAllInstructorsQueryHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithDefaultPagination_ShouldReturnPaginatedList()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var query = new GetAllInstructorsQuery(PageNumber: 1, PageSize: 10, SearchTerm: null, CoursesCount: null,
            QuizzesCount: null);

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
        var uniqueSearchTerm = $"UniqueInst_{Guid.NewGuid()}";

        var instructor1 = InstructorFactory.CreateInstructor(
            personalInformation: PersonalInformationFactory.CreatePersonalInformation(
                name: uniqueSearchTerm,
                email: $"instructor_{Guid.NewGuid()}@example.com")).Value;

        var instructor2 = InstructorFactory.CreateInstructor(
            personalInformation: PersonalInformationFactory.CreatePersonalInformation(
                name: "Another Instructor Name",
                email: $"instructor_{Guid.NewGuid()}@example.com")).Value;

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            dbContext.Instructors.AddRange(instructor1, instructor2);
            await dbContext.SaveChangesAsync(CancellationToken.None);
        }

        var query = new GetAllInstructorsQuery(PageNumber: 1, PageSize: 10, SearchTerm: uniqueSearchTerm,
            CoursesCount: null, QuizzesCount: null);

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().ContainSingle(i => i.Id == instructor1.Id);
        result.Value.Items.Should().NotContain(i => i.Id == instructor2.Id);
    }

    [Fact]
    public async Task Handle_WithCoursesCountAndQuizzesCount_ShouldFilterCorrectly()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        var instructorEmpty = InstructorFactory.CreateInstructor(
            personalInformation: PersonalInformationFactory.CreatePersonalInformation(
                name: $"InstEmpty_{Guid.NewGuid()}",
                email: $"instructor_{Guid.NewGuid()}@example.com")).Value;

        var instructorActive = InstructorFactory.CreateInstructor(
            personalInformation: PersonalInformationFactory.CreatePersonalInformation(
                name: $"InstActive_{Guid.NewGuid()}",
                email: $"instructor_{Guid.NewGuid()}@example.com")).Value;

        var course = CourseFactory.CreateCourse(instructorId: instructorActive.Id).Value;
        var quiz = QuizFactory.CreateQuiz(courseId: course.Id, instructorId: instructorActive.Id).Value;

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            dbContext.Instructors.AddRange(instructorEmpty, instructorActive);
            dbContext.Courses.Add(course);
            dbContext.Quizzes.Add(quiz);
            await dbContext.SaveChangesAsync(CancellationToken.None);
        }

        var queryEmpty = new GetAllInstructorsQuery(PageNumber: 1, PageSize: 10, SearchTerm: null, CoursesCount: 0,
            QuizzesCount: 0);
        var queryActive = new GetAllInstructorsQuery(PageNumber: 1, PageSize: 10, SearchTerm: null, CoursesCount: 1,
            QuizzesCount: 1);

        // Act
        var resultEmpty = await mediator.Send(queryEmpty);
        var resultActive = await mediator.Send(queryActive);

        // Assert
        resultEmpty.IsSuccess.Should().BeTrue();
        resultEmpty.Value.Items.Any(i => i.Id == instructorEmpty.Id).Should().BeTrue();
        resultEmpty.Value.Items.Any(i => i.Id == instructorActive.Id).Should().BeFalse();

        resultActive.IsSuccess.Should().BeTrue();
        resultActive.Value.Items.Any(i => i.Id == instructorActive.Id).Should().BeTrue();
        resultActive.Value.Items.Any(i => i.Id == instructorEmpty.Id).Should().BeFalse();
    }
}
