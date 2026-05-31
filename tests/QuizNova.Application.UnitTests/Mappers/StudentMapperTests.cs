using QuizNova.Application.Features.Students.Mappers;
using QuizNova.Tests.Common.Users.Students;

using Xunit;

namespace QuizNova.Application.UnitTests.Mappers;

public class StudentMapperTests
{
    [Fact]
    public void ToStudentDto_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var student = StudentFactory.CreateStudent().Value;
        const int enrolledCoursesCount = 3;

        // Act
        var dto = student.ToStudentDto(enrolledCoursesCount);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(student.Id, dto.Id);
        Assert.Equal(student.PersonalInformation.Name, dto.Name);
        Assert.Equal(student.PersonalInformation.Email, dto.Email);
        Assert.Equal(student.PersonalInformation.PhoneNumber, dto.PhoneNumber);
        Assert.Equal(enrolledCoursesCount, dto.EnrolledCoursesCount);
    }
}
