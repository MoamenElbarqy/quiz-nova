using FluentValidation;

namespace QuizNova.Application.Features.Instructors.Commands.UpdateInstructor;

public sealed class UpdateInstructorCommandValidator : AbstractValidator<UpdateInstructorCommand>
{
    public UpdateInstructorCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Instructor ID is required.");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters.");

        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(command => command.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Length(7, 15).WithMessage("Phone number must be between 7 and 15 characters.");
    }
}
