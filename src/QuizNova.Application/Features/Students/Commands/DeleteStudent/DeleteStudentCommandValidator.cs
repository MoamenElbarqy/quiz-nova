using FluentValidation;

namespace QuizNova.Application.Features.Students.Commands.DeleteStudent;

public sealed class DeleteStudentCommandValidator : AbstractValidator<DeleteStudentCommand>
{
    public DeleteStudentCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}
