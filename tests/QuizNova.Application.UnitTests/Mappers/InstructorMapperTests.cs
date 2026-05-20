using QuizNova.Application.Features.Instructors.Mappers;
using QuizNova.Tests.Common.Users.Instructors;

using Xunit;

namespace QuizNova.Application.UnitTests.Mappers;

public class InstructorMapperTests
{
    [Fact]
    public void ToInstructorDto_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var instructor = InstructorFactory.CreateInstructor().Value;
        const int coursesCount = 5;
        const int quizzesCount = 10;

        // Act
        var dto = instructor.ToInstructorDto(coursesCount, quizzesCount);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(instructor.Id, dto.InstructorId);
        Assert.Equal(instructor.PersonalInformation.Name, dto.Name);
        Assert.Equal(instructor.PersonalInformation.Email, dto.Email);
        Assert.Equal(instructor.PersonalInformation.Password, dto.Password);
        Assert.Equal(instructor.PersonalInformation.PhoneNumber, dto.PhoneNumber);
        Assert.Equal(coursesCount, dto.CoursesCount);
        Assert.Equal(quizzesCount, dto.QuizzesCount);
}
}
