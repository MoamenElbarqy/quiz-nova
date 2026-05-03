using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Domain.Entities.Quizzes.Questions.Base;
using QuizNova.Domain.Entities.Quizzes.Questions.Mcq;
using QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

namespace QuizNova.Application.Features.Quizzes.Mappers;

public static class QuestionMapper
{
    public static QuestionDto ToQuestionDto(this Question question)
    {
        return question switch
        {
            Mcq mcq => new McqDto
            {
                Id = mcq.Id,
                QuizId = mcq.QuizId,
                QuestionText = mcq.QuestionText,
                Marks = mcq.Marks,
                NumberOfChoices = mcq.NumberOfChoices,
                CorrectChoiceId = mcq.CorrectChoiceId,
                Choices = mcq.Choices
                    .OrderBy(choice => choice.DisplayOrder)
                    .Select(choice => new ChoiceDto
                    {
                        Id = choice.Id,
                        QuestionId = choice.QuestionId,
                        Text = choice.Text,
                        DisplayOrder = choice.DisplayOrder,
                    })
                    .ToArray(),
            },

            Tf tf => new TfDto
            {
                Id = tf.Id,
                QuizId = tf.QuizId,
                QuestionText = tf.QuestionText,
                Marks = tf.Marks,
                CorrectChoice = tf.CorrectChoice,
            },

            _ => throw new InvalidOperationException(
                $"Unknown question type: {question.GetType().Name}"),
        };
    }
}
