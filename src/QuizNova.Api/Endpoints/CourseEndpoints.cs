using MediatR;

using Microsoft.AspNetCore.Authorization;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Mappers;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Courses.Commands.DeleteCourseById;
using QuizNova.Application.Features.Courses.Commands.UpdateCourseInstructor;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Application.Features.Courses.Queries.GetAllCourses;
using QuizNova.Application.Features.Courses.Queries.GetCourseById;
using QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesById;
using QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesCount;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Endpoints;

public static class CourseEndpoints
{
    public static IEndpointRouteBuilder MapCourseEndpoints(this IEndpointRouteBuilder app)
    {
        var coursesGroup = app.MapGroup("courses")
            .RequireAuthorization()
            .RequireRateLimiting("Global")
            .WithTags("courses")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        var instructorGroup = app.MapGroup("instructor")
            .RequireAuthorization()
            .RequireRateLimiting("Global")
            .WithTags("courses")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // GET courses
        coursesGroup.MapGet(string.Empty, async (ISender sender, [AsParameters] GetAllCoursesQuery query) =>
        {
            var result = await sender.Send(query);
            return result.ToOk();
        })
        .WithName("GetCourses")
        .WithSummary("Retrieves courses.")
        .WithDescription("Returns a paginated and filterable list of courses.")
        .CacheOutput(policy => policy.Tag("courses"))
        .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Admin) })
        .Produces<PaginatedList<CourseDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        // GET courses/{id:guid}
        coursesGroup.MapGet("{id:guid}", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new GetCourseByIdQuery(id));
            return result.ToOk();
        })
        .WithName("GetCourseById")
        .WithSummary("Retrieves a course by its unique identifier.")
        .WithDescription("Fetches the details of a specific course using its ID.")
        .CacheOutput(policy => policy.Tag("courses"))
        .Produces<CourseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // POST courses
        coursesGroup.MapPost(string.Empty, async (ISender sender, CreateCourseRequest request) =>
        {
            var command = request.ToCommand();
            var result = await sender.Send(command);
            return result.ToOk();
        })
        .WithName("CreateCourse")
        .WithSummary("Creates a course.")
        .WithDescription("Creates a course with an optional instructor assignment.")
        .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Instructor) })
        .Produces<CourseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        // PATCH courses/{courseId:guid}/instructor
        coursesGroup.MapPatch("{courseId:guid}/instructor", async (ISender sender, Guid courseId, UpdateCourseInstructorRequest request) =>
        {
            var result = await sender.Send(new UpdateCourseInstructorCommand(courseId, request.InstructorId));
            return result.ToOk();
        })
        .WithName("UpdateCourseInstructor")
        .WithSummary("Updates a course instructor.")
        .WithDescription("Assigns or clears the instructor for a course.")
        .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Admin) })
        .Produces<CourseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // DELETE courses/{id:guid}
        coursesGroup.MapDelete("{id:guid}", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new DeleteCourseByIdCommand(id));
            return result.ToNoContent();
        })
        .WithName("DeleteCourseById")
        .WithSummary("Deletes a course by its unique identifier.")
        .WithDescription("Removes a course from the database using its ID.")
        .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Admin) })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // GET instructor/{instructorId:guid}/courses
        instructorGroup.MapGet("{instructorId:guid}/courses", async (ISender sender, Guid instructorId) =>
        {
            var result = await sender.Send(new GetInstructorCoursesByIdQuery(instructorId));
            return result.ToOk();
        })
        .WithName("GetInstructorCourses")
        .WithSummary("Retrieves instructor courses.")
        .WithDescription("Returns all courses for a specific instructor.")
        .CacheOutput(policy => policy.Tag("courses"))
        .Produces<List<CourseDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // GET instructor/{instructorId:guid}/courses/count
        instructorGroup.MapGet("{instructorId:guid}/courses/count", async (ISender sender, Guid instructorId) =>
        {
            var result = await sender.Send(new GetInstructorCoursesCountQuery(instructorId));
            return result.ToOk();
        })
        .WithName("GetInstructorCoursesCount")
        .WithSummary("Retrieves instructor course counts.")
        .WithDescription("Returns instructor course counts based on the instructor ID.")
        .CacheOutput(policy => policy.Tag("courses"))
        .Produces<CoursesCountDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
