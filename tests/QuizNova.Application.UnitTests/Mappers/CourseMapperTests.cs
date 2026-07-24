using QuizNova.Application.Features.Courses.Mappers;
using QuizNova.Tests.Common.Courses;
using QuizNova.Tests.Common.Users.Instructors;

using Xunit;

namespace QuizNova.Application.UnitTests.Mappers;

public class CourseMapperTests
{
    [Fact]
    public void ToCourseDto_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var instructor = InstructorFactory.CreateInstructor().Value;
        var course = CourseFactory.CreateCourse(instructorId: instructor.Id).Value;

        // Act
        var dto = course.ToCourseDto("Dr. John", 0, 0, course.MaximumMarks);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(course.Id, dto.Id);
        Assert.Equal(course.Name, dto.CourseName);
        Assert.Equal(course.InstructorId, dto.InstructorId);
        Assert.Equal(0, dto.EnrolledStudentsCount);
        Assert.Equal(0, dto.QuizzesCount);
        Assert.Equal(course.MaximumMarks, dto.RemainingMarks);
    }

    [Fact]
    public void ToCourseDto_WithNullInstructor_TheNameShouldBeNull()
    {
        // Arrange
        var course = CourseFactory.CreateCourse(instructorId: null).Value;

        // Act
        var dto = course.ToCourseDto(null, 0, 0, course.MaximumMarks);

        // Assert
        Assert.NotNull(dto);
        Assert.Null(dto.InstructorName);
    }
}
