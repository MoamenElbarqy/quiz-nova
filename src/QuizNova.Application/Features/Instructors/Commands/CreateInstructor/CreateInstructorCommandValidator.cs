using FluentValidation;

using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.Features.Instructors.Commands.CreateInstructor;

public sealed class CreateInstructorCommandValidator : AbstractValidator<CreateInstructorCommand>
{
    public CreateInstructorCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters.");

        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(command => command.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(command => command.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Length(7, 15).WithMessage("Phone number must be between 7 and 15 characters.");

        RuleFor(command => command.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(role => string.Equals(role, nameof(UserRole.Instructor), StringComparison.OrdinalIgnoreCase))
            .WithMessage($"Role must be '{UserRole.Instructor}'.");
    }
}
