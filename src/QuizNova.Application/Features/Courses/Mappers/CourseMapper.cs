using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Entities.Courses;

namespace QuizNova.Application.Features.Courses.Mappers;

public static class CourseMapper
{
    public static CourseDto ToCourseDto(this Course course)
    {
        ArgumentNullException.ThrowIfNull(course);

        var instructorName = course.Instructor?.PersonalInformation.Name ?? string.Empty;
        var quizCount = course.Quizzes.Count();

        return new CourseDto(
            course.Id,
            course.Name,
            instructorName,
            EnrolledStudentCount: 0,
            quizCount);
    }
}
