using MediatR;

using Microsoft.Extensions.Logging;

using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Auth.DTOs;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(
    ILogger<GetUserByIdQueryHandler> logger,
    IIdentityService identityService)
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var getUserByIdResult = await identityService.GetUserByIdAsync(request.UserId);

        if (getUserByIdResult.IsError)
        {
            logger.LogError(
                "User with Id { UserId }{ErrorDetails}",
                request.UserId,
                getUserByIdResult.TopError.Description);

            return getUserByIdResult.Errors;
        }

        return getUserByIdResult.Value;
    }
}
