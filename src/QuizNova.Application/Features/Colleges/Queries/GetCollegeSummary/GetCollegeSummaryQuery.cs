using MediatR;

using QuizNova.Application.Features.Colleges.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Colleges.Queries.GetCollegeSummary;

public sealed record GetCollegeSummaryQuery() : IRequest<Result<CollegeSummaryDto>>;
