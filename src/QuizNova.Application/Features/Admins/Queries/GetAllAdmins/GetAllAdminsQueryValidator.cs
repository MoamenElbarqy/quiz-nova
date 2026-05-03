using FluentValidation;

namespace QuizNova.Application.Features.Admins.Queries.GetAllAdmins;

public sealed class GetAllAdminsQueryValidator : AbstractValidator<GetAllAdminsQuery>
{
    public GetAllAdminsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0);

        RuleFor(query => query.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(query => query.SearchTerm)
            .MaximumLength(200)
            .When(query => !string.IsNullOrWhiteSpace(query.SearchTerm));
    }
}
