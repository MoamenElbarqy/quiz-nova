using FluentValidation;

using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.Features.Admin.Commands.CreateAdmin;

public sealed class CreateAdminCommandValidator : AbstractValidator<CreateAdminCommand>
{
    public CreateAdminCommandValidator()
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
            .Must(role => string.Equals(role, UserRole.Admin.ToString(), StringComparison.OrdinalIgnoreCase))
            .WithMessage($"Role must be '{UserRole.Admin}'.");
    }
}
