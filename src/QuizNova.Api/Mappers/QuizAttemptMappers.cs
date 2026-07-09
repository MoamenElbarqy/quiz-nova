using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuestionAnswer;

namespace QuizNova.Api.Mappers;

public static class QuizAttemptMappers
{
    public static SubmitQuestionAnswerCommand ToCommand(this SubmitQuestionAnswerRequest request, Guid attemptId)
    {
        SubmitAnswerCommand answer = request switch
        {
            SubmitMcqAnswerRequest mcq => new SubmitMcqAnswerCommand(
                mcq.QuestionId, mcq.SelectedChoiceId),
            SubmitTfAnswerRequest tf => new SubmitTfAnswerCommand(
                tf.QuestionId, tf.StudentChoice),
            SubmitEssayAnswerRequest essay => new SubmitEssayAnswerCommand(
                essay.QuestionId, essay.StudentResponse),
            _ => throw new InvalidOperationException("Unknown answer type"),
        };

        return new SubmitQuestionAnswerCommand(attemptId, answer);
    }

}
