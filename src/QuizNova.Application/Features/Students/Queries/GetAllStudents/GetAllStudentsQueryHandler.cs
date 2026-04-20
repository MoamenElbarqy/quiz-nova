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
                student.PersonalInformation.Email,
                student.PersonalInformation.Password,
                student.PersonalInformation.PhoneNumber,
                dbContext.StudentCourses.Count(studentCourse => studentCourse.StudentId == student.Id)))
            .ToListAsync(ct);

        return students;
    }
}
