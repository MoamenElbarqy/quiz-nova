using System.Text.Json;

using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Domain.UnitTests.CourseChats;

public class MessageTests
{
    [Fact]
    public void Create_ShouldSuccess_WithValidData()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var senderId = Guid.NewGuid();
        var replyOnId = Guid.NewGuid();
        var content = JsonDocument.Parse("{\"text\":\"hello\"}");

        // Act
        var result = Message.Create(roomId, senderId, replyOnId, content);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal(roomId, result.Value.RoomId);
        Assert.Equal(senderId, result.Value.SenderId);
        Assert.Equal(replyOnId, result.Value.ReplyOnId);
        Assert.Equal("hello", result.Value.Content.RootElement.GetProperty("text").GetString());
        Assert.Empty(result.Value.Reacts);
        Assert.True(DateTimeOffset.UtcNow - result.Value.CreatedAt < TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_ShouldFail_WithEmptyRoomId()
    {
        // Arrange
        var content = JsonDocument.Parse("{}");

        // Act
        var result = Message.Create(Guid.Empty, Guid.NewGuid(), null, content);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Message.RoomIdRequired", result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldFail_WithEmptySenderId()
    {
        // Arrange
        var content = JsonDocument.Parse("{}");

        // Act
        var result = Message.Create(Guid.NewGuid(), Guid.Empty, null, content);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Message.SenderIdRequired", result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldFail_WithNullContent()
    {
        // Act
        var result = Message.Create(Guid.NewGuid(), Guid.NewGuid(), null, null!);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Message.ContentRequired", result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldFail_WithMessageContentTooLong()
    {
        // Arrange
        var content = JsonDocument.Parse("{\"text\":\"" + new string('a', 501) + "\"}");

        // Act
        var result = Message.Create(Guid.NewGuid(), Guid.NewGuid(), null, content);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Message.LengthInvalid", result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldFail_WithMessageContentEmpty()
    {
        // Arrange
        var content = JsonDocument.Parse("{\"text\":\"\"}");

        // Act
        var result = Message.Create(Guid.NewGuid(), Guid.NewGuid(), null, content);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Message.LengthInvalid", result.TopError.Code);
    }

    [Fact]
    public void AddReaction_ShouldAddReaction_WhenNotExists()
    {
        // Arrange
        var message = Message.Create(Guid.NewGuid(), Guid.NewGuid(), null, JsonDocument.Parse("{\"text\":\"hello\"}"))
            .Value;
        var react = React.Create(message.Id, Guid.NewGuid(), "👍").Value;

        // Act
        var result = message.AddReaction(react);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(message.Reacts);
        Assert.Contains(react, message.Reacts);
    }

    [Fact]
    public void RemoveReaction_ShouldRemoveReaction_WhenExists()
    {
        // Arrange
        var message = Message.Create(Guid.NewGuid(), Guid.NewGuid(), null, JsonDocument.Parse("{\"text\":\"hello\"}"))
            .Value;
        var react = React.Create(message.Id, Guid.NewGuid(), "👍").Value;
        message.AddReaction(react);

        // Act
        var result = message.RemoveReaction(react.Id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(message.Reacts);
    }

    [Fact]
    public void RemoveReaction_ShouldFail_WhenNotExists()
    {
        // Arrange
        var message = Message.Create(Guid.NewGuid(), Guid.NewGuid(), null, JsonDocument.Parse("{\"text\":\"hello\"}"))
            .Value;

        // Act
        var result = message.RemoveReaction(Guid.NewGuid());

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("React.ReactionNotFound", result.TopError.Code);
    }

    [Fact]
    public void ReactCreate_ShouldSuccess_WithSurrogatePairEmoji()
    {
        // Act
        var result = React.Create(Guid.NewGuid(), Guid.NewGuid(), "😀");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("😀", result.Value.Emoji);
    }

    [Fact]
    public void ReactCreate_ShouldFail_WithMultipleEmojis()
    {
        // Act
        var result = React.Create(Guid.NewGuid(), Guid.NewGuid(), "👍😀");

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("React.EmojiInvalid", result.TopError.Code);
    }

    [Fact]
    public void ReactCreate_ShouldFail_WithEmptyEmoji()
    {
        // Act
        var result = React.Create(Guid.NewGuid(), Guid.NewGuid(), string.Empty);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("React.EmojiRequired", result.TopError.Code);
    }
}
