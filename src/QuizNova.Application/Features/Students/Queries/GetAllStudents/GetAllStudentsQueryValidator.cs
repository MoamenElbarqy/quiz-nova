using FluentValidation;

using QuizNova.Application.Features.Students.Queries.GetAllStudents;

namespace QuizNova.Application.Features.Students.Queries.GetAllStudents;

public sealed class GetAllStudentsQueryValidator : AbstractValidator<GetAllStudentsQuery>
{
    public GetAllStudentsQueryValidator()
    {
    }
}
