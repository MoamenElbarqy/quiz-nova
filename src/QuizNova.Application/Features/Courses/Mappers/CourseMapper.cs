using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Entities.Courses;

namespace QuizNova.Application.Features.Courses.Mappers;

public static class CourseMapper
{
    public static CourseDto ToCourseDto(this Course course, int enrolledStudentsCount)
    {
        var quizzesCount = course.Quizzes.Count();

        return new CourseDto(
            course.Id,
            course.Name,
            course.InstructorId,
            course.Instructor?.PersonalInformation.Name,
            enrolledStudentsCount,
            quizzesCount,
            course.RemainingMarks);
    }
}
