using MediatR;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Features.Auth.Commands.Login;
using QuizNova.Application.Features.Auth.Commands.RefreshToken;
using QuizNova.Application.Features.Auth.DTOs;

namespace QuizNova.Api.Endpoints;

public static class AuthEndpoints
{
    private const string RefreshTokenCookieName = "refreshToken";

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("auth")
            .AllowAnonymous()
            .WithTags("auth")
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("login", async (ISender sender, HttpContext context, LoginRequest request) =>
        {
            var loginResult = await sender.Send(new LoginCommand(request.Email, request.Password, request.Role));

            return loginResult.Match(
                authResponse =>
                {
                    AppendRefreshTokenCookie(context, authResponse.Token.RefreshToken);
                    return Results.Ok(authResponse);
                },
                ResultExtensions.Problem);
        })
        .WithName("Login")
        .WithSummary("Authenticates a user and issues access tokens.")
        .WithDescription("Validates the provided email and password, then returns an access token response and sets a secure refresh token cookie.")
        .RequireRateLimiting("Auth")
        .Produces<AuthDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("refresh", async (ISender sender, HttpContext context, RefreshTokenRequest request) =>
        {
            var refreshToken = context.Request.Cookies[RefreshTokenCookieName];
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return ResultExtensions.Problem([ApplicationErrors.MissingRefreshToken]);
            }

            var refreshResult = await sender.Send(new RefreshTokenCommand(refreshToken, request.ExpiredAccessToken));

            return refreshResult.Match(
                token =>
                {
                    AppendRefreshTokenCookie(context, token.RefreshToken);
                    return Results.Ok(token);
                },
                ResultExtensions.Problem);
        })
        .WithName("RefreshToken")
        .WithSummary("Refreshes an expired access token.")
        .WithDescription("Validates the refresh token from the secure cookie and returns a rotated token pair.")
        .RequireRateLimiting("Global")
        .Produces<TokenDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }

    private static void AppendRefreshTokenCookie(HttpContext context, string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new InvalidOperationException("Refresh token is required to set authentication cookie.");
        }

        var isHttps = context.Request.IsHttps;
        context.Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/",
            MaxAge = TimeSpan.FromDays(7),
        });
    }
}
