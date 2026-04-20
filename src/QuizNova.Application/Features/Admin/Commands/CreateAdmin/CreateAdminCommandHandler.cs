using MediatR;

using Microsoft.EntityFrameworkCore;

using QuizNova.Application.Common.Errors;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Admin.DTOs;
using QuizNova.Application.Features.Admin.Mappers;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Identity;
using QuizNova.Domain.Entities.Users;
using QuizNova.Domain.Entities.Users.UserPersonalInformation;

namespace QuizNova.Application.Features.Admin.Commands.CreateAdmin;

public sealed class CreateAdminCommandHandler(IAppDbContext dbContext)
    : IRequestHandler<CreateAdminCommand, Result<AdminDto>>
{
    public async Task<Result<AdminDto>> Handle(CreateAdminCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            return ApplicationErrors.UserRoleInvalid(request.Role);
        }

        if (role != UserRole.Admin)
        {
            return ApplicationErrors.CreateAdminRoleInvalid(request.Role);
        }

        if (await dbContext.Entities.OfType<User>().AnyAsync(user => user.Id == request.Id, ct))
        {
            return ApplicationErrors.UserIdAlreadyExists(request.Id);
        }

        if (await dbContext.Entities
                .OfType<User>()
                .AnyAsync(user => user.PersonalInformation.Email == request.Email, ct))
        {
            return ApplicationErrors.UserEmailAlreadyExists(request.Email);
        }

        if (await dbContext.Entities
                .OfType<User>()
                .AnyAsync(user => user.PersonalInformation.PhoneNumber == request.PhoneNumber, ct))
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

        var createAdminResult = Domain.Entities.Users.Admin.Create(
            request.Id,
            personalInformationResult.Value,
            new List<RefreshToken>());

        if (createAdminResult.IsError)
        {
            return createAdminResult.TopError;
        }

        await dbContext.Admins.AddAsync(createAdminResult.Value, ct);
        await dbContext.SaveChangesAsync(ct);

        return createAdminResult.Value.ToAdminDto();
    }
}
