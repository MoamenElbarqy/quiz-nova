using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;
using QuizNova.Application.Features.Quizzes.Commands.UpdateQuestion;

namespace QuizNova.Api.Mappers;

public static class QuizMappers
{
    public static CreateQuizCommand ToCommand(this CreateQuizRequest request)
    {
        return new CreateQuizCommand(
            request.Title,
            request.CourseId,
            request.StartsAtUtc,
            request.EndsAtUtc,
            request.Questions
                .Select(q => q.ToCommand())
                .ToList());
    }

    public static CreateQuestionCommand ToCommand(this CreateQuizQuestionRequest request)
    {
        return request switch
        {
            CreateMcqRequest mcq => new CreateMcqCommand(
                mcq.QuestionText,
                mcq.Marks,
                mcq.CorrectChoiceId,
                mcq.Choices.Select(c => new CreateChoiceCommand(
                        c.Id,
                        c.Text,
                        c.DisplayOrder))
                    .ToList()),
            CreateTfRequest tfq => new CreateTfCommand(
                tfq.QuestionText,
                tfq.Marks,
                tfq.CorrectChoice),
            CreateEssayRequest essay => new CreateEssayCommand(
                essay.QuestionText,
                essay.Marks,
                essay.AnswerReference),
            _ => throw new InvalidOperationException("Unknown question type")
        };
    }

    public static UpdateQuestionCommand ToCommand(this UpdateQuestionRequest request, Guid quizId, Guid questionId)
    {
        return request switch
        {
            UpdateMcqRequest mcq => new UpdateMcqCommand(
                quizId,
                questionId,
                mcq.QuestionText,
                mcq.DisplayOrder,
                mcq.Marks,
                mcq.CorrectChoiceId,
                mcq.Choices.Select(c => new CreateChoiceCommand(
                        c.Id,
                        c.Text,
                        c.DisplayOrder))
                    .ToList()),
            UpdateTfRequest tf => new UpdateTfCommand(
                quizId,
                questionId,
                tf.QuestionText,
                tf.DisplayOrder,
                tf.Marks,
                tf.CorrectChoice),
            UpdateEssayRequest essay => new UpdateEssayCommand(
                quizId,
                questionId,
                essay.QuestionText,
                essay.DisplayOrder,
                essay.Marks,
                essay.AnswerReference),
            _ => throw new InvalidOperationException("Unknown question type")
        };
    }
}
