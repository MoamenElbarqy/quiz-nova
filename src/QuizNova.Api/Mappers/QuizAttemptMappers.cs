using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuizAttempt;

namespace QuizNova.Api.Mappers;

public static class QuizAttemptMappers
{
    public static SubmitQuizAttemptCommand ToCommand(this SubmitQuizAttemptRequest request, Guid studentId)
    {
        return new SubmitQuizAttemptCommand(
            studentId,
            request.QuizId,
            request.StartedAt,
            request.SubmittedAt,
            request.QuestionAnswers
                .Select<SubmitQuestionAnswerRequest, SubmitQuestionAnswerCommand>(answer =>
                {
                    return answer switch
                    {
                        SubmitMcqAnswerRequest mcqAnswer => new SubmitMcqAnswerCommand(
                            mcqAnswer.QuestionId,
                            mcqAnswer.SelectedChoiceId),
                        SubmitTfAnswerRequest tfAnswer => new SubmitTfAnswerCommand(
                            tfAnswer.QuestionId,
                            tfAnswer.StudentChoice),
                        SubmitEssayAnswerRequest essayAnswer => new SubmitEssayAnswerCommand(
                            essayAnswer.QuestionId,
                            essayAnswer.StudentResponse),
                        _ => throw new InvalidOperationException("Unknown answer type"),
                    };
                })
                .ToList());
    }
}
