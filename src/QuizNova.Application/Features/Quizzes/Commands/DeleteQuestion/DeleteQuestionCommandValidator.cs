using FluentValidation;

namespace QuizNova.Application.Features.Quizzes.Commands.DeleteQuestion;

public sealed class DeleteQuestionCommandValidator : AbstractValidator<DeleteQuestionCommand>
{
    public DeleteQuestionCommandValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty().WithMessage("Quiz ID is required.");

        RuleFor(x => x.QuestionId)
            .NotEmpty().WithMessage("Question ID is required.");
    }
}
