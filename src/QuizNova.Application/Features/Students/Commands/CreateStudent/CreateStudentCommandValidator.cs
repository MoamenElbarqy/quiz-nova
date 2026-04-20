using FluentValidation;

using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Application.Features.Students.Commands.CreateStudent;

public sealed class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
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
            .Must(role => string.Equals(role, UserRole.Student.ToString(), StringComparison.OrdinalIgnoreCase))
            .WithMessage($"Role must be '{UserRole.Student}'.");
    }
}
