using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

namespace QuizNova.Api.Mappers;

public static class QuizMappers
{
    public static CreateQuizCommand ToCommand(this CreateQuizRequest request)
    {
        return new CreateQuizCommand(
            request.Title,
            request.CourseId,
            request.InstructorId,
            request.StartsAtUtc,
            request.EndsAtUtc,
            request.Questions
                .Select<CreateQuizQuestionRequest, CreateQuestionCommand>(q =>
                {
                    return q switch
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
                })
                .ToList());
    }
}
