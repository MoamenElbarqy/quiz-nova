using FluentValidation;

namespace QuizNova.Application.Features.Courses.Queries.GetAllCourses;

public sealed class GetAllCoursesQueryValidator : AbstractValidator<GetAllCoursesQuery>
{
    public GetAllCoursesQueryValidator()
    {
    }
}
