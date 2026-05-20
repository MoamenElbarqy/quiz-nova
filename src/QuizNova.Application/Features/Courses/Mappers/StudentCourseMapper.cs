using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Entities.Enrollments;

namespace QuizNova.Application.Features.Courses.Mappers;

public static class EnrollmentMapper
{
    public static EnrollmentDto ToEnrollmentDto(this Enrollment enrollment)
    {
        var course = enrollment.Course;
        var instructorName = course?.Instructor?.PersonalInformation.Name ?? string.Empty;
        var quizzesCount = course?.Quizzes.Count() ?? 0;

        return new EnrollmentDto(
            enrollment.CourseId,
            course?.Name ?? string.Empty,
            instructorName,
            quizzesCount,
            enrollment.EnrolledOnUtc);
    }
}
