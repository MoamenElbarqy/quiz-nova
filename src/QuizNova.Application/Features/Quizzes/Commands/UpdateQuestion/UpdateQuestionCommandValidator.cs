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
            .MaximumLength(500).WithMessage("Question text must not exceed 500 characters.");

        RuleFor(x => x.Marks)
            .GreaterThan(0).WithMessage("Marks must be greater than 0.");

        When(x => x is UpdateMcqCommand, () =>
        {
            RuleFor(x => ((UpdateMcqCommand)x).Choices)
                .NotEmpty().WithMessage("Choices are required for MCQ.")
                .Must(x => x.Count >= 2).WithMessage("MCQ must have at least 2 choices.");

            RuleForEach(x => ((UpdateMcqCommand)x).Choices)
                .SetValidator(new CreateChoiceCommandValidator());

            RuleFor(x => ((UpdateMcqCommand)x).CorrectChoiceId)
                .NotEmpty().WithMessage("Correct choice ID is required for MCQ.")
                .Must((cmd, id) => ((UpdateMcqCommand)cmd).Choices.Any(c => c.Id == id))
                .WithMessage("Correct choice ID must match one of the choices.");
        });
    }
}
