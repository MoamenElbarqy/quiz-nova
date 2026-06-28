using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Courses.Enums;
using QuizNova.Domain.Entities.Courses.Events;
using QuizNova.Tests.Common.Courses;
using QuizNova.Tests.Common.Users.Students;

namespace QuizNova.Domain.UnitTests.Courses;

public class CourseTests
{
    [Fact]
    public void Create_ShouldSuccess_WithValidData()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var name = "Database Systems";

        // Act
        var result = CourseFactory.CreateCourse(
            instructorId: instructorId,
            name: name,
            minimumPassingMarks: 50,
            maximumMarks: 100);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal(instructorId, result.Value.InstructorId);
        Assert.Equal("Database Systems", result.Value.Name);
        Assert.Equal(50, result.Value.MinimumPassingMarks);
        Assert.Equal(100, result.Value.MaximumMarks);
        Assert.Equal(CourseStatus.Active, result.Value.Status);

        var createdEvent = Assert.Single(result.Value.DomainEvents);
        var courseCreatedEvent = Assert.IsType<CourseCreatedEvent>(createdEvent);
        Assert.Equal(result.Value.Id, courseCreatedEvent.Id);
    }

    [Fact]
    public void Create_ShouldSuccess_WithNullInstructorId()
    {
        // Act
        var result = CourseFactory.CreateCourse(instructorId: null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.InstructorId);
    }

    [Fact]
    public void Create_ShouldFail_WithEmptyInstructorId()
    {
        // Act
        var result = CourseFactory.CreateCourse(instructorId: Guid.Empty);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.InstructorIdRequired, result.TopError);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ShouldFail_WithEmptyNameOrNull(string? name)
    {
        // Act
        var result = CourseFactory.CreateCourse(name: name!);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.NameRequired, result.TopError);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    [InlineData("    ab    ")]
    [InlineData("This Course Name Is Way Too Long To Be Valid Because It Exceeds Thirty Characters")]
    public void Create_ShouldFail_WithInvalidName(string name)
    {
        // Act
        var result = CourseFactory.CreateCourse(name: name);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.NameInvalid, result.TopError);
    }

    [Fact]
    public void Create_ShouldTrimName()
    {
        // Act
        var result = CourseFactory.CreateCourse(name: "   Database Systems   ");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Database Systems", result.Value.Name);
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(0)]
    public void Create_ShouldFail_WithNegativeOrZeroMinimumPassingMarks(int minMarks)
    {
        // Act
        var result = CourseFactory.CreateCourse(minimumPassingMarks: minMarks);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.MinimumPassingMarksInvalid, result.TopError);
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(0)]
    public void Create_ShouldFail_WithNegativeOrZeroMaximumMarks(int maxMarks)
    {
        // Act
        var result = CourseFactory.CreateCourse(maximumMarks: maxMarks);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.MaximumMarksInvalid, result.TopError);
    }

    [Fact]
    public void Create_ShouldFail_WithMinimumPassingMarksGreaterThanMaximumMarks()
    {
        // Act
        var result = CourseFactory.CreateCourse(minimumPassingMarks: 60, maximumMarks: 50);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.ScoringRangeInvalid, result.TopError);
    }

    [Fact]
    public void Create_ShouldSuccess_WithMinimumPassingMarksEqualsMaximumMarks()
    {
        // Act
        var result = CourseFactory.CreateCourse(minimumPassingMarks: 50, maximumMarks: 50);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(50, result.Value.MinimumPassingMarks);
        Assert.Equal(50, result.Value.MaximumMarks);
    }

    [Fact]
    public void Create_ShouldSuccess_WithMinimumPassingMarksLessThanMaximumMarks()
    {
        // Act
        var result = CourseFactory.CreateCourse(minimumPassingMarks: 40, maximumMarks: 100);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(40, result.Value.MinimumPassingMarks);
        Assert.Equal(100, result.Value.MaximumMarks);
    }

    [Fact]
    public void Enroll_ShouldSuccess_WhenCourseActive()
    {
        // Arrange
        var course = CourseFactory.CreateCourse().Value;
        var student = StudentFactory.CreateStudent().Value;
        course.ClearDomainEvents();

        // Act
        var result = course.Enroll(student);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Contains(course.Enrollments, e => e.StudentId == student.Id);

        var enrolledEvent = Assert.Single(course.DomainEvents);
        var studentEnrolledEvent = Assert.IsType<StudentEnrolledEvent>(enrolledEvent);
        Assert.Equal(course.Id, studentEnrolledEvent.CourseId);
        Assert.Equal(student.Id, studentEnrolledEvent.StudentId);
    }

    [Fact]
    public void Enroll_ShouldFail_WhenStudentAlreadyEnrolled()
    {
        // Arrange
        var course = CourseFactory.CreateCourse().Value;
        var student = StudentFactory.CreateStudent().Value;
        course.Enroll(student);

        // Act
        var result = course.Enroll(student);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.StudentAlreadyEnrolled(student.Id).Code, result.TopError.Code);
    }

    [Fact]
    public void Enroll_ShouldFail_WhenCourseCompleted()
    {
        // Arrange
        var course = CourseFactory.CreateCourse().Value;
        var student = StudentFactory.CreateStudent().Value;
        course.MarkAsCompeleted();

        // Act
        var result = course.Enroll(student);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.CannotEnrollInCompletedCourse, result.TopError);
    }

    [Fact]
    public void MarkAsCompleted_ShouldSuccess()
    {
        // Arrange
        var course = CourseFactory.CreateCourse().Value;
        course.ClearDomainEvents();

        // Act
        var result = course.MarkAsCompeleted();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(CourseStatus.Completed, course.Status);

        Assert.Empty(course.DomainEvents);
    }

    [Fact]
    public void UpdateInstructor_ShouldSuccess_WhenCourseActive()
    {
        // Arrange
        var course = CourseFactory.CreateCourse().Value;
        var newInstructorId = Guid.NewGuid();
        course.ClearDomainEvents();

        // Act
        var result = course.UpdateInstructor(newInstructorId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newInstructorId, course.InstructorId);

        var updatedEvent = Assert.Single(course.DomainEvents);
        var courseUpdatedEvent = Assert.IsType<CourseUpdatedEvent>(updatedEvent);
        Assert.Equal(course.Id, courseUpdatedEvent.Id);
    }

    [Fact]
    public void UpdateInstructor_ShouldFail_WhenCourseCompleted()
    {
        // Arrange
        var course = CourseFactory.CreateCourse().Value;
        course.MarkAsCompeleted();

        // Act
        var result = course.UpdateInstructor(Guid.NewGuid());

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.CannotUpdateCompletedCourse, result.TopError);
    }

    [Fact]
    public void Delete_ShouldSuccess()
    {
        // Arrange
        var course = CourseFactory.CreateCourse().Value;
        course.ClearDomainEvents();

        // Act
        var result = course.Delete();

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Empty(course.DomainEvents);
    }
}
