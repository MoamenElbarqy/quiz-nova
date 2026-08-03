using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Colleges.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Courses;
using QuizNova.Domain.Entities.Users.Instructors;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Application.Features.Colleges.Queries.GetCollegeSummary;

public sealed class GetCollegeSummaryQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetCollegeSummaryQueryHandler> logger)
    : IRequestHandler<GetCollegeSummaryQuery, Result<CollegeSummaryDto>>
{
    public async Task<Result<CollegeSummaryDto>> Handle(GetCollegeSummaryQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving college summary");

        var totalStudents = (int)await mongoContext.Users.CountDocumentsAsync(
            u => u is Student, cancellationToken: ct);

        var totalInstructors = (int)await mongoContext.Users.CountDocumentsAsync(
            u => u is Instructor, cancellationToken: ct);

        var totalCourses = (int)await mongoContext.Courses.CountDocumentsAsync(
            Builders<Course>.Filter.Empty, cancellationToken: ct);

        var summaryDto = new CollegeSummaryDto(
            totalStudents,
            totalInstructors,
            totalCourses);

        logger.LogInformation(
            "College summary retrieved: {StudentCount} students, {InstructorCount} instructors, {CourseCount} courses",
            summaryDto.TotalStudents,
            summaryDto.TotalInstructors,
            summaryDto.TotalCourses);

        return summaryDto;
    }
}
