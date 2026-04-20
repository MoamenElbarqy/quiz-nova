using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Admin.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Admin.Queries.GetAllAdmins;

public sealed class GetAllAdminsQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetAllAdminsQuery, Result<List<AdminDto>>>
{
    public async Task<Result<List<AdminDto>>> Handle(GetAllAdminsQuery request, CancellationToken ct)
    {
        var admins = await dbContext.Admins
            .AsNoTracking()
            .Select(admin => new AdminDto(
                admin.Id,
                admin.PersonalInformation.Name,
                admin.PersonalInformation.Email,
                admin.PersonalInformation.Password,
                admin.PersonalInformation.PhoneNumber))
            .ToListAsync(ct);

        return admins;
    }
}
