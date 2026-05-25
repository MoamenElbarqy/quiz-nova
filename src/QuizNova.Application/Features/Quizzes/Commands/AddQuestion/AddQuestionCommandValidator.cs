using FluentValidation;

using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

namespace QuizNova.Application.Features.Quizzes.Commands.AddQuestion;

public sealed class AddQuestionCommandValidator : AbstractValidator<AddQuestionCommand>
{
    public AddQuestionCommandValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty().WithMessage("Quiz ID is required.");

        RuleFor(x => x.Question)
            .NotNull().WithMessage("Question is required.");

        RuleFor(x => x.Question).Custom((question, ctx) =>
        {
            var validationResult = question switch
            {
                CreateMcqCommand mcq => new CreateMcqCommandValidator().Validate(mcq),
                CreateTfCommand tf => new CreateTfCommandValidator().Validate(tf),
                CreateEssayCommand essay => new CreateEssayCommandValidator().Validate(essay),
                _ => null,
            };

            if (validationResult is null)
            {
                return;
            }

            foreach (var failure in validationResult.Errors)
            {
                ctx.AddFailure(failure);
            }
        });
    }
}
