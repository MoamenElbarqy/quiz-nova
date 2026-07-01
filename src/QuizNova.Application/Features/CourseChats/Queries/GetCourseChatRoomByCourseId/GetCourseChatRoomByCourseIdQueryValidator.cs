using FluentValidation;

namespace QuizNova.Application.Features.CourseChats.Queries.GetCourseChatRoomByCourseId;

public sealed class GetCourseChatRoomByCourseIdQueryValidator : AbstractValidator<GetCourseChatRoomByCourseIdQuery>
{
    public GetCourseChatRoomByCourseIdQueryValidator()
    {
        RuleFor(query => query.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");
    }
}
