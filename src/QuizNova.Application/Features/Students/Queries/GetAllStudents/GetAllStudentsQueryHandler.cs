using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Students.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Students.Queries.GetAllStudents;

public sealed class GetAllStudentsQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetAllStudentsQuery, Result<List<StudentDto>>>
{
    public async Task<Result<List<StudentDto>>> Handle(GetAllStudentsQuery request, CancellationToken ct)
    {
        var students = await dbContext.Students
            .AsNoTracking()
            .Select(student => new StudentDto(
                student.Id,
                student.PersonalInformation.Name,
                dbContext.Departments
                    .Where(department => department.Id == student.DepartmentId)
                    .Select(department => department.Name)
                    .FirstOrDefault() ?? string.Empty,
                dbContext.Levels
                    .Where(level => level.Id == student.LevelId)
                    .Select(selector: level => level.Name)
                    .FirstOrDefault() ?? string.Empty,
                dbContext.StudentCourses.Count(studentCourse => studentCourse.StudentId == student.Id)))
            .OrderBy(student => student.Name)
            .ToListAsync(ct);

        return students;
    }
}
