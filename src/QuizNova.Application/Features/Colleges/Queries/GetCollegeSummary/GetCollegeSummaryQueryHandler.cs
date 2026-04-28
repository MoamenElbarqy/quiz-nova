using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Colleges.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Colleges.Queries.GetCollegeSummary;

public sealed class GetCollegeSummaryQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetCollegeSummaryQueryHandler> logger)
    : IRequestHandler<GetCollegeSummaryQuery, Result<CollegeSummaryDto>>
{
    public Task<Result<CollegeSummaryDto>> Handle(GetCollegeSummaryQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving college summary");

        var summaryDto = new CollegeSummaryDto(
            dbContext.Students.Count(),
            dbContext.Instructors.Count(),
            dbContext.Courses.Count());

        logger.LogInformation(
            "College summary retrieved: {StudentCount} students, {InstructorCount} instructors, {CourseCount} courses",
            summaryDto.TotalStudents,
            summaryDto.TotalInstructors,
            summaryDto.TotalCourses);

        return Task.FromResult<Result<CollegeSummaryDto>>(summaryDto);
    }
}
