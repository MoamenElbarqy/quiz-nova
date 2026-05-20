using FluentValidation;

using QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

namespace QuizNova.Application.Features.Quizzes.Commands.UpdateQuestion;

public sealed class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionCommandValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty().WithMessage("Quiz ID is required.");

        RuleFor(x => x.QuestionId)
            .NotEmpty().WithMessage("Question ID is required.");

        RuleFor(x => x.QuestionText)
            .NotEmpty().WithMessage("Question text is required.")
            .MinimumLength(3).WithMessage("Question text must be at least 3 characters long.")
            .MaximumLength(500).WithMessage("Question text must not exceed 500 characters.");

        RuleFor(x => x.Marks)
            .GreaterThan(0).WithMessage("Marks must be greater than 0.");
    }
}

public sealed class UpdateMcqCommandValidator : AbstractValidator<UpdateMcqCommand>
{
    public UpdateMcqCommandValidator()
    {
        Include(new UpdateQuestionCommandValidator());

        RuleFor(x => x.Choices)
            .NotEmpty().WithMessage("Choices are required for MCQ.")
            .Must(x => x.Count >= 2).WithMessage("MCQ must have at least 2 choices.");

        RuleForEach(x => x.Choices)
            .SetValidator(new CreateChoiceCommandValidator());

        RuleFor(x => x.CorrectChoiceId)
            .NotEmpty().WithMessage("Correct choice ID is required for MCQ.")
            .Must((cmd, id) => cmd.Choices.Any(c => c.Id == id))
            .WithMessage("Correct choice ID must match one of the choices.");
    }
}

public sealed class UpdateTfCommandValidator : AbstractValidator<UpdateTfCommand>
{
    public UpdateTfCommandValidator()
    {
        Include(new UpdateQuestionCommandValidator());
    }
}
