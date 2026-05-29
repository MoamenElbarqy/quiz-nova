using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Features.Courses.Commands.CreateCourse;

namespace QuizNova.Api.Mappers;

public static class CourseMappers
{
    public static CreateCourseCommand ToCommand(this CreateCourseRequest request) =>
        new(request.Name, request.InstructorId, request.MinimumPassingMarks, request.MaximumMarks);
}
