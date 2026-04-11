using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;

public static class QuestionMapper
{
    public static QuestionDto ToQuestionDto(this Question question)
    {
        return new QuestionDto
        {
            Id = question.Id,
            QuizId = question.QuizId,
            QuestionText = question.QuestionText,
            Marks = question.Marks,
        };
    }
}
