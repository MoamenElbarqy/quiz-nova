using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Mappers;
using QuizNova.Domain.Entities.Quizzes.Questions.AutoGradedQuestions.Mcq.Choices;
using QuizNova.Tests.Common.Quizzes.Questions;

using Xunit;

namespace QuizNova.Application.UnitTests.Mappers;

public class QuestionMapperTests
{
    [Fact]
    public void ToQuestionDto_WithTfQuestion_ShouldMapCorrectly()
    {
        // Arrange
        var tfQuestion = QuestionFactory.CreateTfQuestion(
            questionText: "Test TF Question",
            correctChoice: true,
            displayOrder: 2,
            marks: 8).Value;

        // Act
        var dto = tfQuestion.ToQuestionDto();

        // Assert
        Assert.NotNull(dto);
        var tfDto = Assert.IsType<TfDto>(dto);
        Assert.Equal(tfQuestion.Id, tfDto.Id);
        Assert.Equal(tfQuestion.QuizId, tfDto.QuizId);
        Assert.Equal(tfQuestion.QuestionText, tfDto.QuestionText);
        Assert.Equal(tfQuestion.Marks, tfDto.Marks);
        Assert.Equal(tfQuestion.CorrectChoice, tfDto.CorrectChoice);
}

    [Fact]
    public void ToQuestionDto_WithMcqQuestion_ShouldMapCorrectly()
    {
        // Arrange
        var choice1 = ChoiceFactory.CreateChoice(text: "Choice 1", displayOrder: 2).Value;
        var choice2 = ChoiceFactory.CreateChoice(text: "Choice 2", displayOrder: 1).Value;
        var choices = new List<Choice> { choice1, choice2 };

        var mcqQuestion = QuestionFactory.CreateMcqQuestion(
            questionText: "Test MCQ Question",
            correctChoiceId: choice1.Id,
            displayOrder: 1,
            marks: 10,
            choices: choices).Value;

        // Act
        var dto = mcqQuestion.ToQuestionDto();

        // Assert
        Assert.NotNull(dto);
        var mcqDto = Assert.IsType<McqDto>(dto);
        Assert.Equal(mcqQuestion.Id, mcqDto.Id);
        Assert.Equal(mcqQuestion.QuizId, mcqDto.QuizId);
        Assert.Equal(mcqQuestion.QuestionText, mcqDto.QuestionText);
        Assert.Equal(mcqQuestion.Marks, mcqDto.Marks);
        Assert.Equal(mcqQuestion.NumberOfChoices, mcqDto.NumberOfChoices);
        Assert.Equal(mcqQuestion.CorrectChoiceId, mcqDto.CorrectChoiceId);

        // Assert choices ordered by DisplayOrder
        Assert.NotNull(mcqDto.Choices);
        Assert.Equal(2, mcqDto.Choices.Count);

        // Choice 2 has DisplayOrder = 1, so it should be first
        Assert.Equal(choice2.Id, mcqDto.Choices.ElementAt(0).Id);
        Assert.Equal(choice2.Text, mcqDto.Choices.ElementAt(0).Text);
        Assert.Equal(choice2.DisplayOrder, mcqDto.Choices.ElementAt(0).DisplayOrder);

        // Choice 1 has DisplayOrder = 2, so it should be second
        Assert.Equal(choice1.Id, mcqDto.Choices.ElementAt(1).Id);
        Assert.Equal(choice1.Text, mcqDto.Choices.ElementAt(1).Text);
        Assert.Equal(choice1.DisplayOrder, mcqDto.Choices.ElementAt(1).DisplayOrder);
}
}
