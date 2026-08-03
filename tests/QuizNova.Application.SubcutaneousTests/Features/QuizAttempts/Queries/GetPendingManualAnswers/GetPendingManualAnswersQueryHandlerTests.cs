using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.Queries.GetPendingManualAnswers;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Quizzes.Questions;
using QuizNova.Tests.Common.Courses;
using QuizNova.Tests.Common.QuizAttempts;
using QuizNova.Tests.Common.QuizAttempts.Answers;
using QuizNova.Tests.Common.Quizzes;
using QuizNova.Tests.Common.Security;
using QuizNova.Tests.Common.Users.Instructors;
using QuizNova.Tests.Common.Users.Students;
using QuizNova.Tests.Common.Users.UserPersonalInformation;

namespace QuizNova.Application.SubcutaneousTests.Features.QuizAttempts.Queries.GetPendingManualAnswers;

public class GetPendingManualAnswersQueryHandlerTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_WithDefaultPagination_ShouldReturnPaginatedList()
    {
        // Arrange
        var mediator = factory.CreateMediator();
        var instructorUser = TestUsers.Instructor1.User;
        TestCurrentUser.Set(instructorUser);

        var query = new GetPendingManualAnswersQuery();

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PageNumber.Should().Be(1);
        result.Value.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WithPendingAndScoredAnswers_ShouldReturnOnlyAttemptsWithPendingAnswers()
    {
        // Arrange
        var mediator = factory.CreateMediator();

        // 1. Setup instructor matching the TestCurrentUser
        var instructorId = Guid.Parse(TestUsers.Instructor1.User.Id);
        var instructor = InstructorFactory.CreateInstructor(
            id: instructorId,
            personalInformation: PersonalInformationFactory.CreatePersonalInformation(
                name: "Instructor Manual Grading",
                email: TestUsers.Instructor1.User.Email!)).Value;

        TestCurrentUser.Set(TestUsers.Instructor1.User);

        // 2. Setup Student, Course, Quiz with Manual (Essay) Questions
        var student = StudentFactory.CreateStudent().Value;
        var course = CourseFactory.CreateCourse(instructorId: instructorId).Value;

        var quizId1 = Guid.NewGuid();
        var quizId2 = Guid.NewGuid();

        var questionArgs1 = new List<CreateQuestionArgs>
        {
            new CreateEssayArgs("Question 1", 5, "Ref1"),
            new CreateEssayArgs("Question 2", 5, "Ref2"),
            new CreateEssayArgs("Question 3", 5, "Ref3"),
        };

        var questionArgs2 = new List<CreateQuestionArgs>
        {
            new CreateEssayArgs("Question 4", 5, "Ref4"),
            new CreateEssayArgs("Question 5", 5, "Ref5"),
            new CreateEssayArgs("Question 6", 5, "Ref6"),
        };

        var quizWithPending = QuizFactory.CreateQuiz(id: quizId1, courseId: course.Id, instructorId: instructorId,
            questionArgs: questionArgs1).Value;
        var quizAllScored = QuizFactory.CreateQuiz(id: quizId2, courseId: course.Id, instructorId: instructorId,
            questionArgs: questionArgs2).Value;

        var q1 = quizWithPending.Questions.First();
        var q4 = quizAllScored.Questions.First();

        // 3. Setup Attempts
        // Attempt 1: has unscored essay answer
        var attemptId1 = Guid.NewGuid();
        var unscoredAnswer = AnswerFactory
            .CreateEssayAnswer(studentId: student.Id, questionId: q1.Id, quizAttemptId: attemptId1, score: null).Value;
        var attemptWithPending = QuizAttemptFactory.CreateQuizAttempt(quizId: quizId1,
            id: attemptId1, studentId: student.Id).Value;
        attemptWithPending.SubmitAnswer(unscoredAnswer);

        // Attempt 2: has scored essay answer
        var attemptId2 = Guid.NewGuid();
        var scoredAnswer = AnswerFactory
            .CreateEssayAnswer(studentId: student.Id, questionId: q4.Id, quizAttemptId: attemptId2, score: 5).Value;
        var attemptAllScored = QuizAttemptFactory.CreateQuizAttempt(quizId: quizId2,
            id: attemptId2, studentId: student.Id).Value;
        attemptAllScored.SubmitAnswer(scoredAnswer);

        using (var scope = factory.Services.CreateScope())
        {
            var mongoContext = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
            await mongoContext.Users.InsertOneAsync(instructor);
            await mongoContext.Users.InsertOneAsync(student);
            await mongoContext.Courses.InsertOneAsync(course);

            await mongoContext.Quizzes.InsertManyAsync([quizWithPending, quizAllScored]);
            await mongoContext.QuizAttempts.InsertManyAsync([attemptWithPending, attemptAllScored]);
        }

        var query = new GetPendingManualAnswersQuery();

        // Act
        var result = await mediator.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Any(a => a.AttemptId == attemptWithPending.Id).Should().BeTrue();
        result.Value.Items.Any(a => a.AttemptId == attemptAllScored.Id).Should().BeFalse();
    }
}
