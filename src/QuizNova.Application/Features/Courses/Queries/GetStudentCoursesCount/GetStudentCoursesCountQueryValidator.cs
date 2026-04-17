using FluentValidation;

namespace QuizNova.Application.Features.Courses.Queries.GetStudentCoursesCount;

public sealed class GetStudentCoursesCountQueryValidator : AbstractValidator<GetStudentCoursesCountQuery>
{
    public GetStudentCoursesCountQueryValidator()
    {
        RuleFor(query => query.StudentId)
            .NotEmpty();
    }
}

