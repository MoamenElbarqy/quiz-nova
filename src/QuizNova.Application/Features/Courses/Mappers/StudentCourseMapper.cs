using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Entities.StudentCourses;

namespace QuizNova.Application.Features.Courses.Mappers;

public static class StudentCourseMapper
{
    public static StudentCourseDto ToStudentCourseDto(this StudentCourse studentCourse)
    {
        ArgumentNullException.ThrowIfNull(studentCourse);

        var course = studentCourse.Course;
        var instructorName = course?.Instructor?.PersonalInformation.Name ?? string.Empty;
        var quizzesCount = course?.Quizzes.Count() ?? 0;

        return new StudentCourseDto(
            studentCourse.CourseId,
            course?.Name ?? string.Empty,
            instructorName,
            quizzesCount,
            studentCourse.EnrolledOnUtc);
    }
}
