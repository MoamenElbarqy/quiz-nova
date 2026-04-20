using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Admin.DTOs;
using QuizNova.Application.Features.Admin.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Admin.Commands.UpdateAdmin;

public sealed class UpdateAdminCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<UpdateAdminCommand, Result<AdminDto>>
{
    public async Task<Result<AdminDto>> Handle(UpdateAdminCommand request, CancellationToken ct)
    {
        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, ct);

        if (admin is null)
        {
            return ApplicationErrors.AdminNotFound(request.Id);
        }

        if (await dbContext.Entities
                .OfType<User>()
                .AnyAsync(user => user.Id != request.Id && user.PersonalInformation.Email == request.Email, ct))
        {
            return ApplicationErrors.UserEmailAlreadyExists(request.Email);
        }

        if (await dbContext.Entities
                .OfType<User>()
                .AnyAsync(
                    user => user.Id != request.Id && user.PersonalInformation.PhoneNumber == request.PhoneNumber,
                    ct))
        {
            return ApplicationErrors.UserPhoneNumberAlreadyExists(request.PhoneNumber);
        }

        var personalInformationResult = PersonalInformation.Create(
            request.Name,
            request.Email,
            request.Password,
            request.PhoneNumber);

        if (personalInformationResult.IsError)
        {
            return personalInformationResult.TopError;
        }

        var updateAdminResult = admin.Update(personalInformationResult.Value);

        if (updateAdminResult.IsError)
        {
            return updateAdminResult.TopError;
        }

        dbContext.Admins.Update(admin);
        await dbContext.SaveChangesAsync(ct);

        return admin.ToAdminDto();
    }
}
