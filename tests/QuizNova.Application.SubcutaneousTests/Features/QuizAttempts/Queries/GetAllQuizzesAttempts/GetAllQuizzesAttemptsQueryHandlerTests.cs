using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.Queries.GetAllQuizzesAttempts;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Tests.Common.Courses;
using QuizNova.Tests.Common.QuizAttempts;
using QuizNova.Tests.Common.QuizAttempts.Answers;
using QuizNova.Tests.Common.Quizzes;
using QuizNova.Tests.Common.Users.Instructors;
using QuizNova.Tests.Common.Users.Students;
using QuizNova.Tests.Common.Users.UserPersonalInformation;

namespace QuizNova.Application.SubcutaneousTests.Features.QuizAttempts.Queries.GetAllQuizzesAttempts;

public class GetAllQuizzesAttemptsQueryHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithDefaultPagination_ShouldReturnPaginatedList()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var query = new GetAllQuizzesAttemptsQuery();

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

        var student1 = StudentFactory.CreateStudent(
            personalInformation: PersonalInformationFactory.CreatePersonalInformation(
                name: $"StudentOne_{Guid.NewGuid()}",
                email: $"student_{Guid.NewGuid()}@example.com")).Value;

        var student2 = StudentFactory.CreateStudent(
            personalInformation: PersonalInformationFactory.CreatePersonalInformation(
                name: "StudentTwo",
                email: $"student_{Guid.NewGuid()}@example.com")).Value;

        var instructor = InstructorFactory.CreateInstructor().Value;
        var course = CourseFactory.CreateCourse(instructorId: instructor.Id).Value;
        var quiz = QuizFactory.CreateQuiz(courseId: course.Id, instructorId: instructor.Id).Value;

        var attempt1 = QuizAttemptFactory.CreateQuizAttempt(studentId: student1.Id, quizId: quiz.Id).Value;
        var attempt2 = QuizAttemptFactory.CreateQuizAttempt(studentId: student2.Id, quizId: quiz.Id).Value;

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            dbContext.Students.AddRange(student1, student2);
            dbContext.Instructors.Add(instructor);
            dbContext.Courses.Add(course);
            dbContext.Quizzes.Add(quiz);
            dbContext.QuizAttempts.AddRange(attempt1, attempt2);
            await dbContext.SaveChangesAsync(CancellationToken.None);
        }

        var query = new GetAllQuizzesAttemptsQuery(SearchTerm: student1.PersonalInformation.Name);

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Any(a => a.QuizAttemptId == attempt1.Id).Should().BeTrue();
        result.Value.Items.Any(a => a.QuizAttemptId == attempt2.Id).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithCorrectAnswersFilter_ShouldFilterCorrectly()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        var student = StudentFactory.CreateStudent().Value;
        var instructor = InstructorFactory.CreateInstructor().Value;
        var course = CourseFactory.CreateCourse(instructorId: instructor.Id).Value;
        var quiz = QuizFactory.CreateQuiz(courseId: course.Id, instructorId: instructor.Id).Value;

        var attemptId = Guid.NewGuid();
        var questionId = quiz.Questions.First().Id;
        var answer1 = AnswerFactory.CreateTfAnswer(studentId: student.Id, questionId: questionId, quizAttemptId: attemptId, isCorrect: true)
            .Value;
        var attempt = QuizAttemptFactory
            .CreateQuizAttempt(id: attemptId, studentId: student.Id, quizId: quiz.Id, studentAnswers: [answer1]).Value;

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            dbContext.Students.Add(student);
            dbContext.Instructors.Add(instructor);
            dbContext.Courses.Add(course);
            dbContext.Quizzes.Add(quiz);
            dbContext.QuizAttempts.Add(attempt);
            await dbContext.SaveChangesAsync(CancellationToken.None);
        }

        var queryMatch = new GetAllQuizzesAttemptsQuery(CorrectAnswers: 1);
        var queryMismatch = new GetAllQuizzesAttemptsQuery(CorrectAnswers: 0);

        // Act
        var resultMatch = await mediator.Send(queryMatch);
        var resultMismatch = await mediator.Send(queryMismatch);

        // Assert
        resultMatch.IsSuccess.Should().BeTrue();
        resultMatch.Value.Items.Any(a => a.QuizAttemptId == attempt.Id).Should().BeTrue();

        resultMismatch.IsSuccess.Should().BeTrue();
        resultMismatch.Value.Items.Any(a => a.QuizAttemptId == attempt.Id).Should().BeFalse();
    }
}
