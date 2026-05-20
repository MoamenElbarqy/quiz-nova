using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Tests.Common.Quizzes.Questions;

using Xunit;

namespace QuizNova.Application.UnitTests.Mappers;

public class ChoiceMapperTests
{
    [Fact]
    public void ToChoiceDto_ShouldMapFieldsCorrectly()
    {
        // Arrange
        var choice = ChoiceFactory.CreateChoice(text: "Some Choice Text", displayOrder: 3).Value;

        // Act
        var dto = choice.ToChoiceDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(choice.Id, dto.Id);
        Assert.Equal(choice.QuestionId, dto.QuestionId);
        Assert.Equal(choice.Text, dto.Text);
        Assert.Equal(choice.DisplayOrder, dto.DisplayOrder);
}
}
