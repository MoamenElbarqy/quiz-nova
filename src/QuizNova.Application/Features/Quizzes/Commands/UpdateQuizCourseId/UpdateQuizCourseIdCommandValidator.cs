using FluentValidation;

namespace QuizNova.Application.Features.Quizzes.Commands.UpdateQuizCourseId;

public sealed class UpdateQuizCourseIdCommandValidator : AbstractValidator<UpdateQuizCourseIdCommand>
{
    public UpdateQuizCourseIdCommandValidator()
    {
        RuleFor(x => x.QuizId)
            .NotEmpty()
            .WithMessage("Quiz ID is required.");

        RuleFor(x => x.NewCourseId)
            .NotEmpty()
            .WithMessage("New Course ID is required.");
    }
}
