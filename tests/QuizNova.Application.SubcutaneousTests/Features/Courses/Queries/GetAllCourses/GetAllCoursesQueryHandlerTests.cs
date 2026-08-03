using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.Queries.GetAllCourses;
using QuizNova.Application.Features.Instructors.Commands.CreateInstructor;
using QuizNova.Application.Features.Users.DTOs;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users.Admins;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;
using QuizNova.Tests.Common.Courses;
using QuizNova.Tests.Common.Enrollments;
using QuizNova.Tests.Common.Quizzes;
using QuizNova.Tests.Common.Security;
using QuizNova.Tests.Common.Users.Instructors;
using QuizNova.Tests.Common.Users.Students;

namespace QuizNova.Application.SubcutaneousTests.Features.Courses.Queries.GetAllCourses;

public class GetAllCoursesQueryHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithDefaultPagination_ShouldReturnPaginatedList()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var query = new GetAllCoursesQuery();

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
        var uniqueSearchTerm = $"Course_{Guid.NewGuid().ToString()[..10]}";

        var course1 = CourseFactory.CreateCourse(name: uniqueSearchTerm).Value;
        var course2 = CourseFactory.CreateCourse(name: "Other Course").Value;

        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
            await mongoContext.Courses.InsertManyAsync([course1, course2]);

        }

        var query = new GetAllCoursesQuery(SearchTerm: uniqueSearchTerm);

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().ContainSingle(c => c.Id == course1.Id);
        result.Value.Items.Should().NotContain(c => c.Id == course2.Id);
    }

    [Fact]
    public async Task Handle_WithInstructorId_ShouldFilterCorrectly()
    {
        // Arrange
        EnsureAdminContext();
        var mediator = factory.CreateMediator();

        var createInstructorCmd1 = new CreateInstructorCommand(
            PersonalInformation: new PersonalInformationDto("Instructor One", $"instructor_{Guid.NewGuid()}@example.com", $"+1{Guid.NewGuid().ToString()[..10]}"),
            Password: "SecurePass123!",
            Role: nameof(UserRole.Instructor));

        var createInstructorCmd2 = new CreateInstructorCommand(
            PersonalInformation: new PersonalInformationDto("Instructor Two", $"instructor_{Guid.NewGuid()}@example.com", $"+1{Guid.NewGuid().ToString()[..10]}"),
            Password: "SecurePass123!",
            Role: nameof(UserRole.Instructor));

        var instructorResult1 = await mediator.Send(createInstructorCmd1);
        var instructorResult2 = await mediator.Send(createInstructorCmd2);

        instructorResult1.IsSuccess.Should().BeTrue();
        instructorResult2.IsSuccess.Should().BeTrue();

        var instructorId = instructorResult1.Value.Id;
        var otherInstructorId = instructorResult2.Value.Id;

        var instructedCourse = CourseFactory.CreateCourse(instructorId: instructorId).Value;
        var otherCourse = CourseFactory.CreateCourse(instructorId: otherInstructorId).Value;

        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
            await mongoContext.Courses.InsertManyAsync([instructedCourse, otherCourse]);

        }

        var query = new GetAllCoursesQuery(InstructorId: instructorId);

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().ContainSingle(c => c.Id == instructedCourse.Id);
        result.Value.Items.Should().NotContain(c => c.Id == otherCourse.Id);
    }

    [Fact]
    public async Task Handle_WithStudentId_ShouldReturnEnrolledCourses()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var student = StudentFactory.CreateStudent().Value;

        var enrolledCourse = CourseFactory.CreateCourse().Value;
        var nonEnrolledCourse = CourseFactory.CreateCourse().Value;
        var enrollment = EnrollmentFactory.CreateEnrollment(studentId: student.Id, courseId: enrolledCourse.Id).Value;

        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
            await mongoContext.Users.InsertOneAsync(student);
            await mongoContext.Courses.InsertManyAsync([enrolledCourse, nonEnrolledCourse]);
            await mongoContext.Enrollments.InsertOneAsync(enrollment);

        }

        var query = new GetAllCoursesQuery(StudentId: student.Id);

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().ContainSingle(c => c.Id == enrolledCourse.Id);
        result.Value.Items.Should().NotContain(c => c.Id == nonEnrolledCourse.Id);
    }

    [Fact]
    public async Task Handle_WithEnrolledStudentsCountAndQuizzesCount_ShouldFilterCorrectly()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var student = StudentFactory.CreateStudent().Value;
        var instructor = InstructorFactory.CreateInstructor().Value;

        var courseActive = CourseFactory.CreateCourse(instructorId: instructor.Id).Value;
        var courseEmpty = CourseFactory.CreateCourse().Value;

        var enrollment = EnrollmentFactory.CreateEnrollment(studentId: student.Id, courseId: courseActive.Id).Value;
        var quiz = QuizFactory.CreateQuiz(course: courseActive, instructorId: instructor.Id).Value;

        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
            await mongoContext.Users.InsertOneAsync(student);
            await mongoContext.Users.InsertOneAsync(instructor);
            await mongoContext.Courses.InsertManyAsync([courseActive, courseEmpty]);
            await mongoContext.Enrollments.InsertOneAsync(enrollment);

            await mongoContext.Quizzes.InsertOneAsync(quiz);
        }

        var queryActive = new GetAllCoursesQuery(EnrolledStudentsCount: 1, QuizzesCount: 1);
        var queryEmpty = new GetAllCoursesQuery(EnrolledStudentsCount: 0, QuizzesCount: 0);

        // Act
        var resultActive = await mediator.Send(queryActive);
        var resultEmpty = await mediator.Send(queryEmpty);

        // Assert
        resultActive.IsSuccess.Should().BeTrue();
        resultActive.Value.Items.Any(c => c.Id == courseActive.Id).Should().BeTrue();
        resultActive.Value.Items.Any(c => c.Id == courseEmpty.Id).Should().BeFalse();

        resultEmpty.IsSuccess.Should().BeTrue();
        resultEmpty.Value.Items.Any(c => c.Id == courseEmpty.Id).Should().BeTrue();
        resultEmpty.Value.Items.Any(c => c.Id == courseActive.Id).Should().BeFalse();
    }

    private void EnsureAdminContext()
    {
        var adminId = Guid.Parse(TestUsers.Admin.User.Id);
        using var scope = factory.Services.CreateScope();
        var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
        if (!mongoContext.Users.Find(u => u.UserRole == UserRole.Admin && u.Id == adminId).Any())
        {
            var personalInfo = PersonalInformation.Create("Admin User", "admin@quiznova.local", "01000000000").Value;
            var admin = Admin.Create(adminId, personalInfo).Value;
            mongoContext.Users.InsertOne(admin);
        }

        TestCurrentUser.Set(TestUsers.Admin.User);
    }
}
