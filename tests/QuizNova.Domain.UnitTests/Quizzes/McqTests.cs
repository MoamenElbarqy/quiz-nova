using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using QuizNova.Tests.Common.Quizzes.Questions;

namespace QuizNova.Domain.UnitTests.Quizzes;

public class McqTests
{
    [Fact]
    public void Create_ShouldSuccess_WithValidData()
    {
        // Act
        var result = QuestionFactory.CreateMcqQuestion(
            questionText: "What is 2 + 2?");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    [InlineData("   a   ")]
    public void Create_ShouldFail_WithTitleTooShort(string questionText)
    {
        // Act
        var result = QuestionFactory.CreateMcqQuestion(
            questionText: questionText);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(McqErrors.TitleTooShort, result.TopError);
    }

    [Fact]
    public void Create_ShouldFail_WithTitleTooLong()
    {
        // Arrange
        var questionText = new string('a', 501);

        // Act
        var result = QuestionFactory.CreateMcqQuestion(
            questionText: questionText);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(McqErrors.TitleTooLong, result.TopError);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    [InlineData("   a   ")]
    public void CreateChoice_ShouldFail_WithChoiceTooShort(string choiceText)
    {
        // Act
        var result = ChoiceFactory.CreateChoice(text: choiceText);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ChoiceErrors.ChoiceTooShort, result.TopError);
    }

    [Fact]
    public void CreateChoice_ShouldFail_WithChoiceTooLong()
    {
        // Arrange
        var choiceText = new string('a', 101);

        // Act
        var result = ChoiceFactory.CreateChoice(text: choiceText);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ChoiceErrors.ChoiceTooLong, result.TopError);
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    [InlineData("   a   ")]
    public void UpdateChoice_ShouldFail_WithChoiceTooShort(string choiceText)
    {
        // Arrange
        var choice = ChoiceFactory.CreateChoice().Value;

        // Act
        var result = choice.Update(choiceText, 1);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ChoiceErrors.ChoiceTooShort, result.TopError);
    }

    [Fact]
    public void UpdateChoice_ShouldFail_WithChoiceTooLong()
    {
        // Arrange
        var choice = ChoiceFactory.CreateChoice().Value;
        var choiceText = new string('a', 101);

        // Act
        var result = choice.Update(choiceText, 1);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ChoiceErrors.ChoiceTooLong, result.TopError);
    }
}
