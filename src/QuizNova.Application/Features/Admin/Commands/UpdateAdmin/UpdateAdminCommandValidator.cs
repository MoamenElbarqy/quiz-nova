using FluentValidation;

namespace QuizNova.Application.Features.Admin.Commands.UpdateAdmin;

public sealed class UpdateAdminCommandValidator : AbstractValidator<UpdateAdminCommand>
{
    public UpdateAdminCommandValidator()
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
    }
}
