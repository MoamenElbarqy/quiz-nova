using System.Globalization;

using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.CourseChats;

public sealed class React : Entity
{
    private React()
    {
    }

    private React(Guid id, Guid messageId, Guid reactorId, string emoji)
        : base(id)
    {
        MessageId = messageId;
        ReactorId = reactorId;
        Emoji = emoji;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid MessageId { get; private set; }

    public Message Message { get; private set; } = null!;

    public Guid ReactorId { get; private set; }

    public User Reactor { get; private set; } = null!;

    public string Emoji { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public static Result<React> Create(Guid messageId, Guid reactorId, string emoji)
    {
        if (messageId == Guid.Empty)
        {
            return CourseChatErrors.MessageIdRequired;
        }

        if (reactorId == Guid.Empty)
        {
            return CourseChatErrors.ReactorIdRequired;
        }

        if (string.IsNullOrWhiteSpace(emoji))
        {
            return CourseChatErrors.EmojiRequired;
        }

        var stringInfo = new StringInfo(emoji);
        if (stringInfo.LengthInTextElements != 1)
        {
            return CourseChatErrors.EmojiInvalid;
        }

        return new React(Guid.NewGuid(), messageId, reactorId, emoji);
    }
}
