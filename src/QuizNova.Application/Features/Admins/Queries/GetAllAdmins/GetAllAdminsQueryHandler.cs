using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Application.Features.Admins.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.Admins;

namespace QuizNova.Application.Features.Admins.Queries.GetAllAdmins;

public sealed class GetAllAdminsQueryHandler(
    IMongoDbContext mongoContext,
    ILogger<GetAllAdminsQueryHandler> logger)
    : IRequestHandler<GetAllAdminsQuery, Result<PaginatedList<AdminDto>>>
{
    public async Task<Result<PaginatedList<AdminDto>>> Handle(GetAllAdminsQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving all admins");

        var filter = Builders<User>.Filter.Where(u => u is Admin);
        filter = ApplySearchTerm(filter, request);

        var totalCount = (int)await mongoContext.Users.CountDocumentsAsync(filter, cancellationToken: ct);

        var admins = await mongoContext.Users
            .Find(filter)
            .SortBy(u => u.PersonalInformation.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Limit(request.PageSize)
            .ToListAsync(ct);

        var adminDtos = admins
            .Cast<Admin>()
            .Select(admin => admin.ToAdminDto())
            .ToList();

        var response = new PaginatedList<AdminDto>(
            adminDtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation("Successfully retrieved {Count} admins for page {PageNumber}", adminDtos.Count,
            request.PageNumber);

        return response;
    }

    private static FilterDefinition<User> ApplySearchTerm(
        FilterDefinition<User> filter,
        GetAllAdminsQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return filter;
        }

        return filter & Builders<User>.Filter.Where(admin =>
            admin.PersonalInformation.Name.Contains(request.SearchTerm) ||
            admin.PersonalInformation.Email.Contains(request.SearchTerm));
    }
}
