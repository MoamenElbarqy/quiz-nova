using FluentValidation;

using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.Features.Admins.Commands.CreateAdmin;

public sealed class CreateAdminCommandValidator : AbstractValidator<CreateAdminCommand>
{
    public CreateAdminCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Admin ID is required.");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(command => command.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(command => command.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.");

        RuleFor(command => command.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(role => string.Equals(role, UserRole.Admin.ToString(), StringComparison.OrdinalIgnoreCase))
            .WithMessage($"Role must be '{UserRole.Admin}'.");
    }
}
