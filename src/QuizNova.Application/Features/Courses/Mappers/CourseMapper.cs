using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Entities.Courses;

namespace QuizNova.Application.Features.Courses.Mappers;

public static class CourseMapper
{
    public static CourseDto ToCourseDto(
        this Course course,
        string? instructorName,
        int enrolledStudentsCount,
        int quizzesCount,
        int remainingMarks)
    {
        return new CourseDto(
            course.Id,
            course.Name,
            course.InstructorId,
            instructorName,
            enrolledStudentsCount,
            quizzesCount,
            remainingMarks);
    }
}
