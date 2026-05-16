using FluentValidation;

namespace QuizNova.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Quiz ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");

        RuleFor(x => x.InstructorId)
            .NotEmpty().WithMessage("Instructor ID is required.");

        RuleFor(x => x.StartsAtUtc)
            .NotEmpty().WithMessage("Start time is required.");

        RuleFor(x => x.EndsAtUtc)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartsAtUtc).WithMessage("End time must be after start time.");

        RuleFor(x => x.Questions)
            .NotEmpty().WithMessage("At least one question is required.");

        RuleForEach(x => x.Questions)
            .SetValidator(new CreateQuestionCommandValidator());
    }
}

public sealed class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Question ID is required.");

        RuleFor(x => x.QuestionText)
            .NotEmpty().WithMessage("Question text is required.")
            .MaximumLength(500).WithMessage("Question text must not exceed 500 characters.");

        RuleFor(x => x.Marks)
            .GreaterThan(0).WithMessage("Marks must be greater than 0.");

        When(x => x is CreateMcqCommand, () =>
        {
            RuleFor(x => ((CreateMcqCommand)x).Choices)
                .NotEmpty().WithMessage("Choices are required for MCQ.")
                .Must(x => x.Count >= 2).WithMessage("MCQ must have at least 2 choices.");

            RuleForEach(x => ((CreateMcqCommand)x).Choices)
                .SetValidator(new CreateChoiceCommandValidator());

            RuleFor(x => ((CreateMcqCommand)x).CorrectChoiceId)
                .NotEmpty().WithMessage("Correct choice ID is required for MCQ.")
                .Must((cmd, id) => ((CreateMcqCommand)cmd).Choices.Any(c => c.Id == id))
                .WithMessage("Correct choice ID must match one of the choices.");
        });
    }
}

public sealed class CreateChoiceCommandValidator : AbstractValidator<CreateChoiceCommand>
{
    public CreateChoiceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Choice ID is required.");

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Choice text is required.")
            .MaximumLength(200).WithMessage("Choice text must not exceed 200 characters.");
    }
}
