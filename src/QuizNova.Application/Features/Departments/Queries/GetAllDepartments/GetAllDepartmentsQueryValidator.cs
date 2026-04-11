using FluentValidation;

namespace QuizNova.Application.Features.Departments.Queries.GetCollegeDepartments;

public sealed class GetAllDepartmentsQueryValidator : AbstractValidator<GetAllDepartmentsQuery>
{
    public GetAllDepartmentsQueryValidator()
    {
    }
}
