using FluentValidation;

namespace QuizNova.Application.Features.Instructor.Queries.GetInstructorById;

public sealed class GetInstructorByIdQueryValidator : AbstractValidator<GetInstructorByIdQuery>
{
    public GetInstructorByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty().WithMessage("Instructor ID is required.");
    }
}
