using QuizNova.Domain.Entities.Quizzes;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Tests.Common.Courses;
using QuizNova.Tests.Common.Quizzes;
using QuizNova.Tests.Common.Quizzes.Questions;

namespace QuizNova.Domain.UnitTests.Quizzes;

public class QuizTests
{
    [Fact]
    public void Create_ShouldSuccess_WithValidData()
    {
        // Arrange
        var id = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        const string title = "Data Structures & Algorithms";
        var starts = DateTimeOffset.UtcNow.AddHours(1);
        var ends = DateTimeOffset.UtcNow.AddHours(3);

        var q1 = QuestionFactory.CreateTfQuestion(quizId: id, displayOrder: 0).Value;
        var q2 = QuestionFactory.CreateTfQuestion(quizId: id, displayOrder: 1).Value;
        var q3 = QuestionFactory.CreateTfQuestion(quizId: id, displayOrder: 2).Value;
        List<Question> questions = [q1, q2, q3];

        // Act
        var result = QuizFactory.CreateQuiz(id, courseId, instructorId, title, starts, ends, questions);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(title, result.Value.Title);
        Assert.Equal(3, result.Value.Questions.Count());

        Assert.Empty(result.Value.DomainEvents);
    }

    [Fact]
    public void Create_ShouldFail_WithEmptyCourseId()
    {
        // Act
        var result = QuizFactory.CreateQuiz(courseId: Guid.Empty);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.CourseIdRequired, result.TopError);
    }

    [Fact]
    public void Create_ShouldFail_WithEmptyInstructorId()
    {
        // Act
        var result = QuizFactory.CreateQuiz(instructorId: Guid.Empty);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.InstructorIdRequired, result.TopError);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldFail_WithEmptyTitle(string title)
    {
        // Act
        var result = QuizFactory.CreateQuiz(title: title);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.TitleRequired, result.TopError);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    [InlineData("   a   ")]
    public void Create_ShouldFail_WithTitleTooShort(string title)
    {
        // Act
        var result = QuizFactory.CreateQuiz(title: title);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.TitleTooShort, result.TopError);
    }

    [Fact]
    public void Create_ShouldTrimTitle()
    {
        // Act
        var result = QuizFactory.CreateQuiz(title: "   Data Structures   ");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Data Structures", result.Value.Title);
    }

    [Fact]
    public void Create_ShouldFail_WithStartsAtGreaterThanOrEqualEndsAt()
    {
        // Arrange
        var starts = DateTimeOffset.UtcNow.AddHours(2);
        var ends = DateTimeOffset.UtcNow.AddHours(1);

        // Act
        var result = QuizFactory.CreateQuiz(startsAtUtc: starts, endsAtUtc: ends);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.ScheduleInvalid, result.TopError);
    }

    [Fact]
    public void Create_ShouldFail_WithScheduleDurationLessThanTenMinutes()
    {
        // Arrange
        var starts = DateTimeOffset.UtcNow.AddHours(1);
        var ends = starts.AddMinutes(9); // Less than 10 minutes!

        // Act
        var result = QuizFactory.CreateQuiz(startsAtUtc: starts, endsAtUtc: ends);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.ScheduleDurationTooShort, result.TopError);
    }

    [Fact]
    public void Update_ShouldFail_WithScheduleDurationLessThanTenMinutes()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;

        // Act
        var result = quiz.Update("Updated Title", DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(1).AddMinutes(9));

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.ScheduleDurationTooShort, result.TopError);
    }

    [Fact]
    public void Create_ShouldFail_WithZeroQuestions()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = QuizFactory.CreateQuiz(
            id: id,
            questions: []);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.QuestionsRequired, result.TopError);
    }

    [Fact]
    public void Create_ShouldFail_WithNonContiguousDisplayOrderSequence()
    {
        // Arrange
        var id = Guid.NewGuid();
        var q1 = QuestionFactory.CreateTfQuestion(quizId: id, displayOrder: 0).Value;
        var q2 = QuestionFactory.CreateTfQuestion(quizId: id, displayOrder: 1).Value;
        var q3 = QuestionFactory.CreateTfQuestion(quizId: id, displayOrder: 3).Value; // 2 is missing!

        // Act
        var result = QuizFactory.CreateQuiz(id: id, questions: [q1, q2, q3]);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.QuestionSequenceInvalid, result.TopError);
    }

    [Fact]
    public void Create_ShouldSuccess_WithNonSequentialContiguousDisplayOrderSequence()
    {
        // Arrange
        var id = Guid.NewGuid();
        var q1 = QuestionFactory.CreateTfQuestion(quizId: id, displayOrder: 2).Value;
        var q2 = QuestionFactory.CreateTfQuestion(quizId: id, displayOrder: 0).Value;
        var q3 = QuestionFactory.CreateTfQuestion(quizId: id, displayOrder: 1)
            .Value; // DisplayOrders are {2, 0, 1} which are contiguous 0 to 2!

        // Act
        var result = QuizFactory.CreateQuiz(id: id, questions: [q1, q2, q3]);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Update_ShouldSuccess_WhenQuizScheduled()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;

        // Act
        var result = quiz.Update("Updated Title", DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(4));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Title", quiz.Title);
    }

    [Fact]
    public void Update_ShouldTrimTitleAndValidateLength()
    {
        // Act
        var quiz = QuizFactory.CreateQuiz().Value;

        // Assert
        var emptyResult = quiz.Update("   ", DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(3));
        Assert.True(emptyResult.IsError);
        Assert.Equal(QuizErrors.TitleRequired, emptyResult.TopError);

        var shortResult = quiz.Update("   ab   ", DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(3));
        Assert.True(shortResult.IsError);
        Assert.Equal(QuizErrors.TitleTooShort, shortResult.TopError);
    }

    [Fact]
    public void Update_ShouldFail_WhenAssociatedCourseCompleted()
    {
        // Arrange
        var course = CourseFactory.CreateCourse().Value;
        course.MarkAsCompeleted();

        var quiz = QuizFactory.CreateQuiz().Value;
        typeof(Quiz).GetProperty("Course")!.SetValue(quiz, course);

        // Act
        var result = quiz.Update("New Title", DateTimeOffset.UtcNow.AddHours(1), DateTimeOffset.UtcNow.AddHours(3));

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.CourseCompleted, result.TopError);
    }

    [Fact]
    public void UpdateCourseId_ShouldSuccess_WhenQuizScheduledAndShouldClearQuestions()
    {
        // Assert
        var quiz = QuizFactory.CreateQuiz().Value;
        Assert.NotEmpty(quiz.Questions);

        var result = quiz.UpdateCourseId(Guid.NewGuid());

        Assert.True(result.IsSuccess);
        Assert.Empty(quiz.Questions);
    }

    [Fact]
    public void UpdateCourseId_ShouldFail_WhenAssociatedCourseCompleted()
    {
        // Arrange
        var course = CourseFactory.CreateCourse().Value;
        course.MarkAsCompeleted();
        var quiz = QuizFactory.CreateQuiz().Value;
        typeof(Quiz).GetProperty("Course")!.SetValue(quiz, course);

        // Act
        var result = quiz.UpdateCourseId(Guid.NewGuid());

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.CourseCompleted, result.TopError);
    }

    [Fact]
    public void AddQuestion_ShouldSuccess_WhenQuizScheduled()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        var newQuestion = QuestionFactory.CreateTfQuestion(quizId: quiz.Id, displayOrder: 3).Value;

        // Act
        var result = quiz.AddQuestion(newQuestion);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(quiz.Questions, q => q.Id == newQuestion.Id);
    }

    [Fact]
    public void AddQuestion_ShouldFail_WhenQuestionBelongsToDifferentQuiz()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        var diffQuizQuestion = QuestionFactory.CreateTfQuestion(quizId: Guid.NewGuid(), displayOrder: 3).Value;

        // Act
        var result = quiz.AddQuestion(diffQuizQuestion);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.QuestionBelongsToDifferentQuiz(diffQuizQuestion.Id).Code, result.TopError.Code);
    }

    [Fact]
    public void AddQuestion_ShouldFail_WhenQuestionAlreadyExists()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        var existingQuestion = quiz.Questions.First();

        // Act
        var result = quiz.AddQuestion(existingQuestion);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.QuestionAlreadyExists(existingQuestion.Id).Code, result.TopError.Code);
    }

    [Fact]
    public void AddQuestion_ShouldFail_WhenAssociatedCourseCompleted()
    {
        // Arrange
        var course = CourseFactory.CreateCourse().Value;
        course.MarkAsCompeleted();
        var quiz = QuizFactory.CreateQuiz().Value;
        typeof(Quiz).GetProperty("Course")!.SetValue(quiz, course);
        var q = QuestionFactory.CreateTfQuestion(quizId: quiz.Id, displayOrder: 3).Value;

        // Act
        var result = quiz.AddQuestion(q);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.CourseCompleted, result.TopError);
    }

    [Fact]
    public void DeleteQuestion_ShouldSuccess_WhenQuizScheduledAndHasMoreThanThreeQuestions()
    {
        // Assert
        var quiz = QuizFactory.CreateQuiz().Value;
        var newQuestion = QuestionFactory.CreateTfQuestion(quizId: quiz.Id, displayOrder: 3).Value;
        quiz.AddQuestion(newQuestion);
        Assert.Equal(4, quiz.Questions.Count());

        var result = quiz.DeleteQuestion(newQuestion);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, quiz.Questions.Count());
    }

    [Fact]
    public void DeleteQuestion_ShouldFail_WhenDeletingLastQuestion()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        Assert.Equal(3, quiz.Questions.Count());
        var questions = quiz.Questions.ToList();

        // Act — delete down to 1 question (should succeed)
        var result1 = quiz.DeleteQuestion(questions[0]);
        var result2 = quiz.DeleteQuestion(questions[1]);
        var result3 = quiz.DeleteQuestion(questions[2]);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.True(result3.IsError);
        Assert.Equal(QuizErrors.MinimumQuestionsReached, result3.TopError);
    }

    [Fact]
    public void DeleteQuestion_ShouldFail_WhenQuestionNotFound()
    {
        // Arrange
        var quiz = QuizFactory.CreateQuiz().Value;
        var notFoundQuestion = QuestionFactory.CreateTfQuestion(quizId: quiz.Id, displayOrder: 3).Value;

        // Act
        var result = quiz.DeleteQuestion(notFoundQuestion);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.QuestionNotFound, result.TopError);
    }

    [Fact]
    public void DeleteQuestion_ShouldFail_WhenAssociatedCourseCompleted()
    {
        // Arrange
        var course = CourseFactory.CreateCourse().Value;
        course.MarkAsCompeleted();
        var quiz = QuizFactory.CreateQuiz().Value;
        typeof(Quiz).GetProperty("Course")!.SetValue(quiz, course);
        var q = quiz.Questions.First();

        // Act
        var result = quiz.DeleteQuestion(q);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.CourseCompleted, result.TopError);
    }

    [Fact]
    public void UpdateQuestion_ShouldFail_WhenAssociatedCourseCompleted()
    {
        // Arrange
        var course = CourseFactory.CreateCourse().Value;
        course.MarkAsCompeleted();
        var quiz = QuizFactory.CreateQuiz().Value;
        typeof(Quiz).GetProperty("Course")!.SetValue(quiz, course);

        // Act
        var result = quiz.UpdateQuestion(
            questionId: Guid.NewGuid(),
            questionText: "Text",
            displayOrder: 1,
            marks: 5);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(QuizErrors.CourseCompleted, result.TopError);
    }
}
