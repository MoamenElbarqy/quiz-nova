using FluentValidation;

using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.Features.Instructor.Commands.CreateInstructor;

public sealed class CreateInstructorCommandValidator : AbstractValidator<CreateInstructorCommand>
{
    public CreateInstructorCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty();

        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(command => command.Password)
            .NotEmpty();

        RuleFor(command => command.PhoneNumber)
            .NotEmpty();

        RuleFor(command => command.Role)
            .NotEmpty()
            .Must(role => string.Equals(role, nameof(UserRole.Instructor), StringComparison.OrdinalIgnoreCase))
            .WithMessage($"Role must be '{UserRole.Instructor}'.");
    }
}
