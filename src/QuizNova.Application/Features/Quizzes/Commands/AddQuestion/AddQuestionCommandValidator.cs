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
            .NotNull().WithMessage("Question is required.")
            .SetValidator(new CreateQuestionCommandValidator());
    }
}
