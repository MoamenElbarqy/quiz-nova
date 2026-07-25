using MediatR;

using Microsoft.AspNetCore.Authorization;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Common.Caching;
using QuizNova.Application.Features.Enrollments.Commands.EnrollStudentInCourse;
using QuizNova.Application.Features.Enrollments.Commands.RemoveStudentFromCourse;
using QuizNova.Application.Features.Enrollments.DTOs;
using QuizNova.Application.Features.Enrollments.Queries.GetAllCoursesEnrollmentCount;
using QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsById;
using QuizNova.Application.Features.Enrollments.Queries.GetStudentEnrollmentsCount;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Endpoints;

public static class EnrollmentEndpoints
{
    public static IEndpointRouteBuilder MapEnrollmentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("courses/enrollments/count", async (ISender sender) =>
            {
                var result = await sender.Send(new GetAllCoursesEnrollmentCountQuery());
                return result.ToOk();
            })
            .WithName("GetAllCoursesEnrollmentCount")
            .WithSummary("Retrieves enrollment counts for all courses.")
            .WithDescription("Returns a list of all courses with their enrollment count, sorted descending. Admin only.")
            .CacheOutput(policy => policy.Tag(CacheTags.Courses).Tag(CacheTags.Enrollments))
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Admin) })
            .RequireRateLimiting(RateLimiterPolicies.Global)
            .WithTags("enrollments")
            .Produces<List<CourseEnrollmentCountDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        var group = app.MapGroup("students")
            .RequireAuthorization()
            .RequireRateLimiting(RateLimiterPolicies.Global)
            .WithTags("enrollments")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("{studentId:guid}/enrollments", async (ISender sender, Guid studentId, EnrollStudentInCourseRequest request) =>
        {
            var result = await sender.Send(new EnrollStudentInCourseCommand(request.CourseId, studentId));
            return result.ToNoContent();
        })
        .WithName("EnrollStudentInCourse")
        .WithSummary("Enrolls a student in a course.")
        .WithDescription("Creates a course enrollment for the specified student.")
        .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Admin) })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("{studentId:guid}/enrollments/{enrollmentId:guid}", async (ISender sender, Guid studentId, Guid enrollmentId) =>
        {
            var result = await sender.Send(new RemoveStudentFromCourseCommand(enrollmentId, studentId));
            return result.ToNoContent();
        })
        .WithName("RemoveStudentFromCourse")
        .WithSummary("Removes a student from a course.")
        .WithDescription("Deletes a course enrollment for the specified student.")
        .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Admin) })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("{studentId:guid}/enrollments", async (ISender sender, Guid studentId) =>
        {
            var result = await sender.Send(new GetStudentEnrollmentsByIdQuery(studentId));
            return result.ToOk();
        })
        .WithName("GetStudentEnrollments")
        .WithSummary("Retrieves student enrollments.")
        .WithDescription("Returns the enrollments for a specific student.")
        .CacheOutput(policy => policy.Tag(CacheTags.Enrollments))
        .Produces<List<EnrollmentDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("{studentId:guid}/enrollments/count", async (ISender sender, Guid studentId) =>
        {
            var result = await sender.Send(new GetStudentEnrollmentsCountQuery(studentId));
            return result.ToOk();
        })
        .WithName("GetStudentEnrollmentsCount")
        .WithSummary("Retrieves student enrollment counts.")
        .WithDescription("Returns student enrollment count based on the student ID.")
        .CacheOutput(policy => policy.Tag(CacheTags.Enrollments))
        .Produces<EnrollmentCountDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
