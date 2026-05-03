using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Common.Models;
using QuizNova.Application.Features.Admins.DTOs;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Admins;

namespace QuizNova.Application.Features.Admins.Queries.GetAllAdmins;

public sealed class GetAllAdminsQueryHandler(
    IAppDbContext dbContext,
    ILogger<GetAllAdminsQueryHandler> logger)
    : IRequestHandler<GetAllAdminsQuery, Result<PaginatedList<AdminDto>>>
{
    public async Task<Result<PaginatedList<AdminDto>>> Handle(GetAllAdminsQuery request, CancellationToken ct)
    {
        logger.LogInformation("Retrieving all admins");

        IQueryable<Admin> query = dbContext.Admins
            .AsNoTracking()
            .AsQueryable();

        query = ApplySearchTerm(query, request);
        query = ApplyFiltering(query, request);
        query = ApplySorting(query);

        var totalCount = await query.CountAsync(ct);

        var admins = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(admin => new AdminDto(
                admin.Id,
                admin.PersonalInformation.Name,
                admin.PersonalInformation.Email,
                admin.PersonalInformation.Password,
                admin.PersonalInformation.PhoneNumber))
            .ToListAsync(ct);

        var response = new PaginatedList<AdminDto>(
            admins,
            totalCount,
            request.PageNumber,
            request.PageSize);

        logger.LogInformation("Successfully retrieved {Count} admins for page {PageNumber}", admins.Count, request.PageNumber);

        return response;
    }

    private static IQueryable<Admin> ApplySearchTerm(
        IQueryable<Admin> query,
        GetAllAdminsQuery request)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return query;
        }

        return query.Where(admin =>
            admin.PersonalInformation.Name.Contains(request.SearchTerm) ||
            admin.PersonalInformation.Email.Contains(request.SearchTerm));
    }

    private static IOrderedQueryable<Admin> ApplySorting(IQueryable<Admin> query)
    {
        return query.OrderBy(admin => admin.PersonalInformation.Name);
    }

    private static IQueryable<Admin> ApplyFiltering(
        IQueryable<Admin> query,
        GetAllAdminsQuery request)
    {
        _ = request;
        return query;
    }
}
