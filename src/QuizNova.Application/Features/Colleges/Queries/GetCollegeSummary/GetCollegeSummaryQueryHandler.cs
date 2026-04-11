using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Colleges.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Colleges.Queries.GetCollegeSummary;

public sealed class GetCollegeSummaryQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetCollegeSummaryQuery, Result<CollegeSummaryDto>>
{
    public async Task<Result<CollegeSummaryDto>> Handle(GetCollegeSummaryQuery request, CancellationToken ct)
    {
        var college = await dbContext.Colleges
            .AsNoTracking()
            .Select(college => new CollegeSummaryDto(
                college.Id,
                college.Name,
                dbContext.Students.Count(),
                dbContext.Instructors.Count(),
                dbContext.Departments.Count(),
                dbContext.Courses.Count()))
            .FirstOrDefaultAsync(ct);

        return college;
    }
}
