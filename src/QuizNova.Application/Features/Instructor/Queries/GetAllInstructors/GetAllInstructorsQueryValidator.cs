using FluentValidation;

namespace QuizNova.Application.Features.Instructor.Queries.GetAllInstructors;

public sealed class GetAllInstructorsQueryValidator : AbstractValidator<GetAllInstructorsQuery>
{
    public GetAllInstructorsQueryValidator()
    {
    }
}
