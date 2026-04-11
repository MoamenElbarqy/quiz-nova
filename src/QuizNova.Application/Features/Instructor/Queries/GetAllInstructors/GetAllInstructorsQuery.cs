using MediatR;

using QuizNova.Application.Features.Instructor.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructor.Queries.GetAllInstructors;

public sealed record GetAllInstructorsQuery()
    : IRequest<Result<List<InstructorDto>>>;
