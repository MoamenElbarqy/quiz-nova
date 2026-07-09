using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuestionAnswer;

public sealed class SubmitQuestionAnswerCommandValidator : AbstractValidator<SubmitQuestionAnswerCommand>
{
    public SubmitQuestionAnswerCommandValidator()
    {
        RuleFor(command => command.AttemptId).NotEmpty().WithMessage("Attempt ID is required.");

        RuleFor(command => command.Answer).NotNull().WithMessage("Answer is required.");

        RuleFor(command => command.Answer.QuestionId).NotEmpty().WithMessage("Question ID is required.");

        RuleFor(command => command.Answer)
            .SetInheritanceValidator(v =>
            {
                v.Add(new SubmitEssayAnswerCommandValidator());
                v.Add(new SubmitMcqAnswerCommandValidator());
            });
    }
}

public sealed class SubmitEssayAnswerCommandValidator : AbstractValidator<SubmitEssayAnswerCommand>
{
    public SubmitEssayAnswerCommandValidator()
    {
        RuleFor(command => command.StudentResponse)
            .NotEmpty().WithMessage("The student response is required.")
            .MinimumLength(3).WithMessage("The student response must be at least 3 characters long.")
            .MaximumLength(1000).WithMessage("The student response must not exceed 1000 characters.");
    }
}

public sealed class SubmitMcqAnswerCommandValidator : AbstractValidator<SubmitMcqAnswerCommand>
{
    public SubmitMcqAnswerCommandValidator()
    {
        RuleFor(command => command.SelectedChoiceId)
            .NotEmpty().WithMessage("Selected choice ID is required.");
    }
}
