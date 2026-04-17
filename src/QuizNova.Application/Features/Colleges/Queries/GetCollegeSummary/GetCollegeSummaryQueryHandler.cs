using MediatR;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Colleges.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Colleges.Queries.GetCollegeSummary;

public sealed class GetCollegeSummaryQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetCollegeSummaryQuery, Result<CollegeSummaryDto>>
{
    public Task<Result<CollegeSummaryDto>> Handle(GetCollegeSummaryQuery request, CancellationToken ct)
    {
        var summaryDto = new CollegeSummaryDto(
            dbContext.Students.Count(),
            dbContext.Instructors.Count(),
            dbContext.Courses.Count());

        return Task.FromResult<Result<CollegeSummaryDto>>(summaryDto);
    }
}
