using MediatR;
using Microsoft.Extensions.Logging;
using QuizNova.Application.Common.Interfaces;
using QuizNova.Application.Features.Identity.Dtos;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Application.Features.Identity.Queries.GetUserInfo;

public class GetUserByIdQueryHanlder(
    ILogger<GetUserByIdQueryHanlder> logger,
    IAuthService authService)
    : IRequestHandler<GetUserByIdQuery, Result<AppUserDto>>
{
    public async Task<Result<AppUserDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var getUserByIdResult = await authService.GetUserByIdAsync(request.UserId!);

        if (getUserByIdResult.IsError)
        {
            logger.LogError("User with Id { UserId }{ErrorDetails}", request.UserId,
                getUserByIdResult.TopError.Description);

            return getUserByIdResult.Errors;
        }

        return getUserByIdResult.Value;
    }
}