using FluentValidation;

namespace QuizNova.Application.Features.Students.Commands.UpdateStudent;

public sealed class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Student ID is required.");

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
