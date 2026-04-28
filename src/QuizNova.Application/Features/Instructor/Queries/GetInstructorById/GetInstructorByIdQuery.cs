using MediatR;

using QuizNova.Application.Features.Instructor.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Instructor.Queries.GetInstructorById;

public sealed record GetInstructorByIdQuery(Guid Id)
    : IRequest<Result<InstructorDto>>;
