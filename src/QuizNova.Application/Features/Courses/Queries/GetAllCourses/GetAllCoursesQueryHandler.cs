using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Courses.Queries.GetAllCourses;

public sealed class GetAllCoursesQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetAllCoursesQuery, Result<List<CourseDto>>>
{
    public async Task<Result<List<CourseDto>>> Handle(GetAllCoursesQuery request, CancellationToken ct)
    {
        var departmentMap = await dbContext.DepartmentCourses
            .AsNoTracking()
            .Join(
                dbContext.Departments.AsNoTracking(),
                departmentCourse => departmentCourse.DepartmentId,
                department => department.Id,
                (departmentCourse, department) => new
                {
                    departmentCourse.CourseId,
                    DepartmentName = department.Name,
                })
            .ToListAsync(ct);

        var courses = await dbContext.Courses
            .AsNoTracking()
            .Select(course => new
            {
                course.Id,
                course.Name,
                InstructorName = dbContext.Instructors
                    .Where(instructor => instructor.Id == course.InstructorId)
                    .Select(instructor => instructor.PersonalInformation.Name)
                    .FirstOrDefault() ?? string.Empty,
                EnrolledStudentCount = dbContext.StudentCourses.Count(studentCourse => studentCourse.CourseId == course.Id),
                QuizCount = dbContext.Quizzes.Count(quiz => quiz.CourseId == course.Id),
            })
            .OrderBy(course => course.Name)
            .ToListAsync(ct);

        var response = courses
            .Select(course => new CourseDto(
                course.Id,
                course.Name,
                departmentMap
                    .Where(item => item.CourseId == course.Id)
                    .Select(item => item.DepartmentName)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList(),
                course.InstructorName,
                course.EnrolledStudentCount,
                course.QuizCount))
            .ToList();

        return response;
    }
}
