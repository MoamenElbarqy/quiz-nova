using FluentValidation;

namespace QuizNova.Application.Features.QuizAttempts.Commands.SubmitQuizAttempt;

public sealed class SubmitQuizAttemptCommandValidator : AbstractValidator<SubmitQuizAttemptCommand>
{
    public SubmitQuizAttemptCommandValidator()
    {
        RuleFor(command => command.StudentId).NotEmpty().WithMessage("Student ID is required.");
        RuleFor(command => command.QuizId).NotEmpty().WithMessage("Quiz ID is required.");
        RuleFor(command => command.StartedAt).NotEqual(default(DateTimeOffset))
            .WithMessage("Started at date is required.");
        RuleFor(command => command.SubmittedAt).NotEqual(default(DateTimeOffset))
            .WithMessage("Submitted at date is required.");

        RuleFor(command => command)
            .Must(command => command.StartedAt < command.SubmittedAt)
            .WithMessage("StartedAt must be before SubmittedAt.");

        RuleFor(command => command.QuestionAnswers)
            .NotEmpty().WithMessage("Question answers are required.");

        RuleFor(command => command.QuestionAnswers)
            .Must(answers => answers.Select(answer => answer.QuestionId).Distinct().Count() == answers.Count)
            .WithMessage("Duplicate question answers are not allowed.");

        RuleForEach(command => command.QuestionAnswers)
            .SetInheritanceValidator(v =>
            {
                v.Add<SubmitEssayAnswerCommand>(new SubmitEssayAnswerCommandValidator());
                v.Add<SubmitMcqAnswerCommand>(new SubmitMcqAnswerCommandValidator());
                v.Add<SubmitTfAnswerCommand>(new SubmitTfAnswerCommandValidator());
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

public sealed class SubmitTfAnswerCommandValidator : AbstractValidator<SubmitTfAnswerCommand>
{
    public SubmitTfAnswerCommandValidator()
    {
    }
}
