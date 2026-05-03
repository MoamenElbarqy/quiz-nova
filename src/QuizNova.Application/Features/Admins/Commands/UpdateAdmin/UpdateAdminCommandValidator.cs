using FluentValidation;

namespace QuizNova.Application.Features.Admins.Commands.UpdateAdmin;

public sealed class UpdateAdminCommandValidator : AbstractValidator<UpdateAdminCommand>
{
    public UpdateAdminCommandValidator()
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
    }
}
