using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.Mappers;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Courses.DTOs;
using QuizNova.Application.Features.Courses.Queries.GetInstructorCoursesPerformance;
using QuizNova.Application.Features.Quizzes.Commands.AddQuestion;
using QuizNova.Application.Features.Quizzes.Commands.DeleteQuestion;
using QuizNova.Application.Features.Quizzes.Commands.UpdateQuizCourseId;
using QuizNova.Application.Features.Quizzes.Commands.UpdateQuizMetadata;
using QuizNova.Application.Features.Quizzes.DTOs;
using QuizNova.Application.Features.Quizzes.Queries.GetAllQuizzes;
using QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzes;
using QuizNova.Application.Features.Quizzes.Queries.GetInstructorQuizzesCount;
using QuizNova.Application.Features.Quizzes.Queries.GetQuizById;
using QuizNova.Application.Features.Quizzes.Queries.GetStudentQuizzes;
using QuizNova.Domain.Entities.Identity;

namespace QuizNova.Api.Endpoints;

public static class QuizEndpoints
{
    public static IEndpointRouteBuilder MapQuizEndpoints(this IEndpointRouteBuilder app)
    {
        var quizzesGroup = app.MapGroup("quizzes")
            .RequireAuthorization()
            .RequireRateLimiting("Global")
            .WithTags("quizzes")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        quizzesGroup.MapGet(string.Empty, async (ISender sender, [AsParameters] GetAllQuizzesQuery query) =>
            {
                var result = await sender.Send(query);
                return result.ToOk();
            })
            .WithName("GetAllQuizzes")
            .WithSummary("Retrieves quizzes.")
            .WithDescription("Returns a paginated and filterable list of quizzes.")
            .CacheOutput(policy => policy.Tag("quizzes"))
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Admin) })
            .Produces<PaginatedList<QuizDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        quizzesGroup.MapGet("count", async (ISender sender, [FromQuery] Guid instructorId) =>
            {
                var result = await sender.Send(new GetInstructorQuizzesCountQuery(instructorId));
                return result.ToOk();
            })
            .WithName("GetInstructorQuizzesCount")
            .WithSummary("Retrieves instructor quiz count.")
            .WithDescription("Returns the number of quizzes created by the specified instructor.")
            .CacheOutput(policy => policy.Tag("quizzes"))
            .RequireAuthorization(new AuthorizeAttribute
            { Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.Instructor)}" })
            .Produces<QuizzesCountDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        quizzesGroup.MapGet("{quizId:guid}", async (ISender sender, Guid quizId) =>
            {
                var result = await sender.Send(new GetQuizByIdQuery(quizId));
                return result.ToOk();
            })
            .WithName("GetQuizById")
            .WithSummary("Retrieves a quiz by id.")
            .WithDescription("Fetches a single quiz using the provided quiz identifier.")
            .CacheOutput(policy => policy.Tag("quizzes"))
            .Produces<QuizDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        quizzesGroup.MapPost(string.Empty, async (ISender sender, CreateQuizRequest request) =>
            {
                var command = request.ToCommand();
                var result = await sender.Send(command);
                return result.ToOk();
            })
            .WithName("CreateQuiz")
            .WithSummary("Creates a new quiz.")
            .WithDescription("Creates a quiz and its question set from the submitted request payload.")
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Instructor) })
            .Produces<QuizDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        quizzesGroup.MapPut("{quizId:guid}/metadata",
                async (ISender sender, Guid quizId, UpdateQuizMetadataRequest request) =>
                {
                    var result = await sender.Send(new UpdateQuizMetadataCommand(
                        quizId,
                        request.Title,
                        request.StartsAtUtc,
                        request.EndsAtUtc));
                    return result.ToNoContent();
                })
            .WithName("UpdateQuizMetadata")
            .WithSummary("Updates quiz metadata.")
            .WithDescription("Updates the title, start time, and end time of an existing quiz.")
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Instructor) })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        quizzesGroup.MapPost("{quizId:guid}/questions",
                async (ISender sender, Guid quizId, CreateQuizQuestionRequest request) =>
                {
                    var result = await sender.Send(new AddQuestionCommand(quizId, request.ToCommand()));
                    return result.ToOk();
                })
            .WithName("AddQuestion")
            .WithSummary("Adds a question to a quiz.")
            .WithDescription("Adds a new MCQ or True/False question to the specified quiz.")
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Instructor) })
            .Produces<QuestionDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        quizzesGroup.MapPut("{quizId:guid}/questions/{questionId:guid}",
                async (ISender sender, Guid quizId, Guid questionId, UpdateQuestionRequest request) =>
                {
                    var result = await sender.Send(request.ToCommand(quizId, questionId));
                    return result.ToNoContent();
                })
            .WithName("UpdateQuestion")
            .WithSummary("Updates a question in a quiz.")
            .WithDescription("Updates an existing MCQ or True/False question within the specified quiz.")
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Instructor) })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        quizzesGroup.MapPut("{quizId:guid}/course",
                async (ISender sender, Guid quizId, UpdateQuizCourseIdRequest request) =>
                {
                    var result = await sender.Send(new UpdateQuizCourseIdCommand(quizId, request.CourseId));
                    return result.ToNoContent();
                })
            .WithName("UpdateQuizCourseId")
            .WithSummary("Updates the course of a quiz.")
            .WithDescription(
                "Changes the course associated with a quiz. This is a destructive operation that clears all existing questions.")
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Instructor) })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        quizzesGroup.MapDelete("{quizId:guid}/questions/{questionId:guid}",
                async (ISender sender, Guid quizId, Guid questionId) =>
                {
                    var result = await sender.Send(new DeleteQuestionCommand(quizId, questionId));
                    return result.ToNoContent();
                })
            .WithName("DeleteQuestion")
            .WithSummary("Deletes a question from a quiz.")
            .WithDescription("Removes a question from the specified quiz. The quiz must have more than 5 questions.")
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Instructor) })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapGet("students/{id:guid}/quizzes", async (ISender sender, Guid id) =>
            {
                var result = await sender.Send(new GetStudentQuizzesQuery(id));
                return result.ToOk();
            })
            .WithName("GetStudentQuizzes")
            .WithSummary("Retrieves quizzes assigned to a student.")
            .WithDescription("Returns quizzes associatined with the specified student identifier.")
            .CacheOutput(policy => policy.Tag("students").Tag("quizzes").SetVaryByQuery("t"))
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Student) })
            .RequireRateLimiting("Global")
            .WithTags("quizzes")
            .Produces<StudentQuizzesDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapGet("instructors/{id:guid}/quizzes", async (ISender sender, Guid id) =>
            {
                var result = await sender.Send(new GetInstructorQuizzesQuery(id));
                return result.ToOk();
            })
            .WithName("GetInstructorQuizzes")
            .WithSummary("Retrieves quizzes created by an instructor.")
            .WithDescription("Returns quizzes associated with the specified instructor identifier.")
            .CacheOutput(policy => policy.Tag("instructors").Tag("quizzes"))
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Instructor) })
            .RequireRateLimiting("Global")
            .WithTags("quizzes")
            .Produces<List<QuizDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapGet("instructors/{instructorId:guid}/courses/performance", async (ISender sender, Guid instructorId) =>
            {
                var result = await sender.Send(new GetInstructorCoursesPerformanceQuery(instructorId));
                return result.ToOk();
            })
            .WithName("GetInstructorCoursesPerformance")
            .WithSummary("Retrieves instructor courses performance.")
            .WithDescription("Returns performance metrics for all courses of a specific instructor.")
            .CacheOutput(policy => policy.Tag("courses").Tag("quizzes").Tag("instructors").Tag("performance"))
            .RequireAuthorization(new AuthorizeAttribute { Roles = nameof(UserRole.Instructor) })
            .RequireRateLimiting("Global")
            .WithTags("quizzes")
            .Produces<List<CoursePerformanceDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}
