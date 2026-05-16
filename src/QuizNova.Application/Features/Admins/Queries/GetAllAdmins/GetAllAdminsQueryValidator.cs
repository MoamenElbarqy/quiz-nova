using FluentValidation;

namespace QuizNova.Application.Features.Admins.Queries.GetAllAdmins;

public sealed class GetAllAdminsQueryValidator : AbstractValidator<GetAllAdminsQuery>
{
    public GetAllAdminsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(query => query.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");

        RuleFor(query => query.SearchTerm)
            .MaximumLength(200).WithMessage("Search term must not exceed 200 characters.")
            .When(query => !string.IsNullOrWhiteSpace(query.SearchTerm));
    }
}
