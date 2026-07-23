using Microsoft.Extensions.Logging;

using NSubstitute;

using QuizNova.Application.Common.Behaviours;
using QuizNova.Application.Common.Interfaces;

using Xunit;

namespace QuizNova.Application.UnitTests.Behaviors;

public class LoggingBehaviourTests
{
    private readonly ILogger<DummyRequest> _logger = Substitute.For<ILogger<DummyRequest>>();
    private readonly IUser _user = Substitute.For<IUser>();
    private readonly IAuthService _authService = Substitute.For<IAuthService>();

    private readonly LoggingBehaviour<DummyRequest> _sut;

    public LoggingBehaviourTests()
    {
        _sut = new LoggingBehaviour<DummyRequest>(_logger, _user, _authService);
    }

    [Fact]
    public async Task Process_WithUserId_LogsRequestWithUserName()
    {
        // Arrange
        var request = new DummyRequest();
        _user.Id.Returns("abc123");
        _authService.GetUserNameAsync("abc123").Returns("Issam");

        // Act
        await _sut.Process(request, CancellationToken.None);

        // Assert
        await _authService.Received(1).GetUserNameAsync("abc123");

        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Request")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task Process_WithoutUserId_LogsRequestWithEmptyUserName()
    {
        // Arrange
        var request = new DummyRequest();
        _user.Id.Returns((string?)null);

        // Act
        await _sut.Process(request, CancellationToken.None);

        // Assert
        await _authService.DidNotReceive().GetUserNameAsync(Arg.Any<string>());

        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Request")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    public class DummyRequest;
}
