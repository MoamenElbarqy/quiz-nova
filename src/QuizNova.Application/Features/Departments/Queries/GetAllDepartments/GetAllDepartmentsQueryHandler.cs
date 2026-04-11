using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Departments.DTOs;
using QuizNova.Application.Features.Departments.Queries.GetCollegeDepartments;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Departments.Queries.GetAllDepartments;

public sealed class GetAllDepartmentsQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetAllDepartmentsQuery, Result<List<DepartmentDto>>>
{
    public async Task<Result<List<DepartmentDto>>> Handle(GetAllDepartmentsQuery request, CancellationToken ct)
    {
        var departments = await dbContext.Departments
            .AsNoTracking()
            .Select(department => new DepartmentDto(
                department.Id,
                department.Name,
                dbContext.Students.Count(student => student.DepartmentId == department.Id),
                dbContext.Courses
                    .Join(
                        dbContext.DepartmentCourses.Where(departmentCourse => departmentCourse.DepartmentId == department.Id),
                        course => course.Id,
                        departmentCourse => departmentCourse.CourseId,
                        (course, _) => course.InstructorId)
                    .Distinct()
                    .Count(),
                dbContext.DepartmentCourses.Count(departmentCourse => departmentCourse.DepartmentId == department.Id)))
            .OrderBy(department => department.DepartmentName)
            .ToListAsync(ct);

        return departments;
    }
}
