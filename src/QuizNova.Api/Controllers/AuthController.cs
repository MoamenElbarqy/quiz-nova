using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizNova.Api.DTOs.Requests;
using QuizNova.Application.Common.Errors;
using QuizNova.Application.Features.Auth.Commands;
using QuizNova.Application.Features.Identity;
using QuizNova.Application.Features.Identity.Dtos;

namespace QuizNova.Api.Controllers;

[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController(ISender sender) : ApiController
{
    private const string RefreshTokenCookieName = "refreshToken";

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Authenticates a user and issues access tokens.")]
    [EndpointDescription("Validates the provided email and password, then returns an access token response and sets a secure refresh token cookie.")]
    [EndpointName("Login")]
    [MapToApiVersion("1.0")]
    [AllowAnonymous]

    public async Task<IActionResult> Login(LoginRequest request)
    {
        var loginResult = await sender.Send(new LoginCommand(request.email, request.password));

        return loginResult.Match(
            authResponse =>
            {
                AppendRefreshTokenCookie(authResponse.Token.RefreshToken);
                return Ok(authResponse);
            },
            Problem);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Refreshes an expired access token.")]
    [EndpointDescription("Validates the refresh token from the secure cookie and returns a rotated token pair.")]
    [EndpointName("RefreshToken")]
    [MapToApiVersion("1.0")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Problem([ApplicationErrors.InvalidRefreshToken]);
        }

        var refreshResult = await sender.Send(new RefreshTokenCommand(refreshToken, request.ExpiredAccessToken));

        return refreshResult.Match(
            token =>
            {
                AppendRefreshTokenCookie(token.RefreshToken);
                return Ok(token);
            },
            Problem);
    }

    private void AppendRefreshTokenCookie(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new InvalidOperationException("Refresh token is required to set authentication cookie.");
        }

        var isHttps = Request.IsHttps;
        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
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
