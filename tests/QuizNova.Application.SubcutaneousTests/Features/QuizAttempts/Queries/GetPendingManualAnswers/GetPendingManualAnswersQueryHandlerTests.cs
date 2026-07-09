using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.QuizAttempts.Queries.GetPendingManualAnswers;
using QuizNova.Application.SubcutaneousTests.Common;
using QuizNova.Domain.Entities.Quizzes.Questions.ManuallyGradedQuestions;
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

        var q1 = Essay.Create(Guid.NewGuid(), quizId1, "Question 1", "Ref1", 0, 5).Value;
        var q2 = Essay.Create(Guid.NewGuid(), quizId1, "Question 2", "Ref2", 1, 5).Value;
        var q3 = Essay.Create(Guid.NewGuid(), quizId1, "Question 3", "Ref3", 2, 5).Value;

        var q4 = Essay.Create(Guid.NewGuid(), quizId2, "Question 4", "Ref4", 0, 5).Value;
        var q5 = Essay.Create(Guid.NewGuid(), quizId2, "Question 5", "Ref5", 1, 5).Value;
        var q6 = Essay.Create(Guid.NewGuid(), quizId2, "Question 6", "Ref6", 2, 5).Value;

        var quizWithPending = QuizFactory.CreateQuiz(id: quizId1, courseId: course.Id, instructorId: instructorId,
            questions: [q1, q2, q3]).Value;
        var quizAllScored = QuizFactory.CreateQuiz(id: quizId2, courseId: course.Id, instructorId: instructorId,
            questions: [q4, q5, q6]).Value;

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
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            dbContext.Instructors.Add(instructor);
            dbContext.Students.Add(student);
            dbContext.Courses.Add(course);
            dbContext.Quizzes.AddRange(quizWithPending, quizAllScored);
            dbContext.QuizAttempts.AddRange(attemptWithPending, attemptAllScored);
            await dbContext.SaveChangesAsync(CancellationToken.None);
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
