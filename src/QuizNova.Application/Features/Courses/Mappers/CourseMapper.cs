using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Entities.Courses;

namespace QuizNova.Application.Features.Courses.Mappers;

public static class CourseMapper
{
    public static CourseDto ToCourseDto(this Course course)
    {
        return new CourseDto(
            course.Id,
            course.CollegeId,
            course.InstructorId,
            course.Name,
            course.MinimumPassingMarks,
            course.MaximumMarks,
            course.IsGraceMarksActivated,
            course.MaxGraceMarks ?? 0);
    }
}
