using FluentValidation;

namespace QuizNova.Application.Features.Admins.Queries.GetAdminById;

public sealed class GetAdminByIdQueryValidator : AbstractValidator<GetAdminByIdQuery>
{
    public GetAdminByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEmpty().WithMessage("Admin ID is required.");
    }
}
