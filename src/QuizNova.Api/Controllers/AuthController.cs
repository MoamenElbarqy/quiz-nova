using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizNova.Api.Mappers;
using QuizNova.Api.DTOs.Requests;
using QuizNova.Api.DTOs.Responses;
using QuizNova.Application.Features.Auth.Commands;

namespace QuizNova.Api.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]

public class AuthController(ISender sender) : ApiController
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Authenticates a user and issues access tokens.")]
    [EndpointDescription("Validates the provided email and password, then returns an access token response and sets a secure refresh token cookie.")]
    [EndpointName("Login")]
    [MapToApiVersion("1.0")]
    [AllowAnonymous]

    public async Task<IActionResult> Login(LoginRequest request)
    {
        var loginResult = await sender.Send(new LoginCommand(request.Email, request.Password));

        return loginResult.Match(
            authResponse =>
            {
                Response.Cookies.Append("refreshToken", authResponse.Token.RefreshToken!, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/",
                    Domain = null,
                    MaxAge = TimeSpan.FromDays(7),
                });
                return Ok(authResponse.ToLoginResponse("Login successful."));
            },
            Problem);
    }
}
