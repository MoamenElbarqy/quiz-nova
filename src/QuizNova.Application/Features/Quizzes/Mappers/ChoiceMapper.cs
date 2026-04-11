using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq.Choices;

public static class ChoiceMapper
{
    public static ChoiceDto ToChoiceDto(this Choice choice)
    {
        return new ChoiceDto
        {
            Id = choice.Id,
            QuestionId = choice.QuestionId,
            Text = choice.Text,
            DisplayOrder = choice.DisplayOrder,
        };
    }
}
