using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Instructor.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructor.Queries.GetAllInstructors;

public sealed class GetAllInstructorsQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetAllInstructorsQuery, Result<List<InstructorDto>>>
{
    public async Task<Result<List<InstructorDto>>> Handle(GetAllInstructorsQuery request, CancellationToken ct)
    {
        var departmentPairs = await dbContext.DepartmentCourses
            .AsNoTracking()
            .Join(
                dbContext.Courses,
                departmentCourse => departmentCourse.CourseId,
                course => course.Id,
                (departmentCourse, course) => new
                {
                    course.InstructorId,
                    departmentCourse.DepartmentId,
                })
            .Distinct()
            .Join(
                dbContext.Departments.AsNoTracking(),
                pair => pair.DepartmentId,
                department => department.Id,
                (pair, department) => new
                {
                    pair.InstructorId,
                    DepartmentName = department.Name,
                })
            .ToListAsync(ct);

        var studentCounts = await dbContext.StudentCourses
            .AsNoTracking()
            .Join(
                dbContext.Courses,
                studentCourse => studentCourse.CourseId,
                course => course.Id,
                (studentCourse, course) => new
                {
                    course.InstructorId,
                    studentCourse.StudentId,
                })
            .Distinct()
            .GroupBy(item => item.InstructorId)
            .Select(group => new
            {
                InstructorId = group.Key,
                StudentCount = group.Count(),
            })
            .ToDictionaryAsync(item => item.InstructorId, item => item.StudentCount, ct);

        var instructors = await dbContext.Instructors
            .AsNoTracking()
            .Select(instructor => new
            {
                instructor.Id,
                instructor.PersonalInformation.Name,
                CourseCount = dbContext.Courses.Count(course => course.InstructorId == instructor.Id),
            })
            .OrderBy(instructor => instructor.Name)
            .ToListAsync(ct);

        var response = instructors
            .Select(instructor => new InstructorDto(
                instructor.Id,
                instructor.Name,
                departmentPairs
                    .Where(pair => pair.InstructorId == instructor.Id)
                    .Select(pair => pair.DepartmentName)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList(),
                instructor.CourseCount,
                studentCounts.GetValueOrDefault(instructor.Id)))
            .ToList();

        return response;
    }
}
