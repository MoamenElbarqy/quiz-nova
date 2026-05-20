using QuizNova.Application.Features.Courses.Mappers;
using QuizNova.Tests.Common.Enrollments;

using Xunit;

namespace QuizNova.Application.UnitTests.Mappers;

public class EnrollmentMapperTests
{
    [Fact]
    public void ToEnrollmentDto_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var enrollment = EnrollmentFactory.CreateEnrollment().Value;

        // Act
        var dto = enrollment.ToEnrollmentDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(enrollment.CourseId, dto.CourseId);
        Assert.Equal(enrollment.EnrolledOnUtc, dto.EnrolledOnUtc);

        Assert.Equal(string.Empty, dto.CourseName);
        Assert.Equal(string.Empty, dto.InstructorName);
        Assert.Equal(0, dto.QuizzesCount);
}
}
