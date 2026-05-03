using FluentValidation;

namespace QuizNova.Application.Features.Admins.Commands.DeleteAdmin;

public sealed class DeleteAdminCommandValidator : AbstractValidator<DeleteAdminCommand>
{
    public DeleteAdminCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Admin ID is required.");
    }
}
