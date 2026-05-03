using FluentValidation;

namespace QuizNova.Application.Features.Instructors.Commands.DeleteInstructor;

public sealed class DeleteInstructorCommandValidator : AbstractValidator<DeleteInstructorCommand>
{
    public DeleteInstructorCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Instructor ID is required.");
    }
}
