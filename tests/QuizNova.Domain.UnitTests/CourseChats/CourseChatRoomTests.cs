using QuizNova.Domain.Entities.CourseChats;
using QuizNova.Tests.Common.Users.Students;

namespace QuizNova.Domain.UnitTests.CourseChats;

public class CourseChatRoomTests
{
    [Fact]
    public void Create_ShouldSuccess_WithValidData()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();

        // Act
        var result = CourseChatRoom.Create(courseId, instructorId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal(courseId, result.Value.CourseId);
        Assert.Equal(instructorId, result.Value.InstructorId);
        Assert.Empty(result.Value.Students);
        Assert.Empty(result.Value.Messages);
    }

    [Fact]
    public void Create_ShouldFail_WithEmptyCourseId()
    {
        // Act
        var result = CourseChatRoom.Create(Guid.Empty, Guid.NewGuid());

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("CourseChatRoom.CourseIdRequired", result.TopError.Code);
    }

    [Fact]
    public void UpdateInstructor_ShouldUpdateInstructorId()
    {
        // Arrange
        var room = CourseChatRoom.Create(Guid.NewGuid(), null).Value;
        var newInstructorId = Guid.NewGuid();

        // Act
        var result = room.UpdateInstructor(newInstructorId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newInstructorId, room.InstructorId);
    }

    [Fact]
    public void UpdateInstructor_ShouldFail_WithEmptyInstructorId()
    {
        // Arrange
        var room = CourseChatRoom.Create(Guid.NewGuid(), null).Value;

        // Act
        var result = room.UpdateInstructor(Guid.Empty);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("CourseChatRoom.InstructorIdRequired", result.TopError.Code);
    }

    [Fact]
    public void AddStudent_ShouldAddStudent()
    {
        // Arrange
        var room = CourseChatRoom.Create(Guid.NewGuid(), null).Value;
        var student = StudentFactory.CreateStudent().Value;

        // Act
        var result = room.AddStudent(student);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(room.Students);
        Assert.Contains(room.Students, s => s.Id == student.Id);
    }

    [Fact]
    public void AddStudent_ShouldFail_WhenStudentAlreadyInRoom()
    {
        // Arrange
        var room = CourseChatRoom.Create(Guid.NewGuid(), null).Value;
        var student = StudentFactory.CreateStudent().Value;
        room.AddStudent(student);

        // Act
        var result = room.AddStudent(student);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("CourseChatRoom.StudentAlreadyInRoom", result.TopError.Code);
    }

    [Fact]
    public void RemoveStudent_ShouldRemoveStudent()
    {
        // Arrange
        var room = CourseChatRoom.Create(Guid.NewGuid(), null).Value;
        var student = StudentFactory.CreateStudent().Value;
        room.AddStudent(student);

        // Act
        var result = room.RemoveStudent(student);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(room.Students);
    }

    [Fact]
    public void RemoveStudent_ShouldFail_WhenStudentNotInRoom()
    {
        // Arrange
        var room = CourseChatRoom.Create(Guid.NewGuid(), null).Value;
        var student = StudentFactory.CreateStudent().Value;

        // Act
        var result = room.RemoveStudent(student);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("CourseChatRoom.StudentNotInRoom", result.TopError.Code);
    }

    [Fact]
    public void CanJoin_ShouldReturnTrue_WhenUserIsInstructor()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var room = CourseChatRoom.Create(Guid.NewGuid(), instructorId).Value;

        // Act
        var result = room.CanJoin(instructorId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanJoin_ShouldReturnTrue_WhenUserIsStudent()
    {
        // Arrange
        var room = CourseChatRoom.Create(Guid.NewGuid(), Guid.NewGuid()).Value;
        var student = StudentFactory.CreateStudent().Value;
        room.AddStudent(student);

        // Act
        var result = room.CanJoin(student.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanJoin_ShouldReturnFalse_WhenUserIsNone()
    {
        // Arrange
        var room = CourseChatRoom.Create(Guid.NewGuid(), Guid.NewGuid()).Value;

        // Act
        var result = room.CanJoin(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanSend_ShouldBeTrueForAnyJoined()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var room = CourseChatRoom.Create(Guid.NewGuid(), instructorId).Value;
        var student = StudentFactory.CreateStudent().Value;
        room.AddStudent(student);

        // Act & Assert
        Assert.True(room.CanSend(instructorId));
        Assert.True(room.CanSend(student.Id));
        Assert.False(room.CanSend(Guid.NewGuid()));
    }

    }
