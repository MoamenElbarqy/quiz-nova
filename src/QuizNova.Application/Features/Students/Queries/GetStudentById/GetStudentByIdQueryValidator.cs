using FluentValidation;

namespace QuizNova.Application.Features.Students.Queries.GetStudentById;

public sealed class GetStudentByIdQueryValidator : AbstractValidator<GetStudentByIdQuery>
{
    public GetStudentByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty().WithMessage("Student ID is required.");
    }
}
