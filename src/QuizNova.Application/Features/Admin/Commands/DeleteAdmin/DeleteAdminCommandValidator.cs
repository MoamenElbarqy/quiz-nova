using FluentValidation;

namespace QuizNova.Application.Features.Admin.Commands.DeleteAdmin;

public sealed class DeleteAdminCommandValidator : AbstractValidator<DeleteAdminCommand>
{
    public DeleteAdminCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}
