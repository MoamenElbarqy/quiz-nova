using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users;

namespace QuizNova.Domain.Entities.CourseChats;

public sealed class Message : Entity
{
    private readonly List<React> _reacts;

    [SetsRequiredMembers]
    private Message()
    {
    }

    [SetsRequiredMembers]
    private Message(
        Guid id,
        Guid roomId,
        Guid senderId,
        Guid? replyOnId,
        DateTimeOffset createdAt,
        JsonDocument content,
        List<React> reacts)
        : base(id)
    {
        RoomId = roomId;
        SenderId = senderId;
        ReplyOnId = replyOnId;
        CreatedAt = createdAt;
        Content = content;
        _reacts = reacts;
    }

    public Guid RoomId { get; private set; }

    public CourseChatRoom Room { get; private set; } = null!;

    public Guid SenderId { get; private set; }

    public User Sender { get; private set; } = null!;

    public Guid? ReplyOnId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public required JsonDocument Content { get; init; }

    public IEnumerable<React> Reacts => _reacts.AsReadOnly();

    public static Result<Message> Create(
        Guid roomId,
        Guid senderId,
        Guid? replyOnId,
        JsonDocument? content)
    {
        if (roomId == Guid.Empty)
        {
            return CourseChatErrors.RoomIdRequired;
        }

        if (senderId == Guid.Empty)
        {
            return CourseChatErrors.SenderIdRequired;
        }

        if (content == null)
        {
            return CourseChatErrors.ContentRequired;
        }

        string? text = null;
        if (content.RootElement.ValueKind == JsonValueKind.String)
        {
            text = content.RootElement.GetString();
        }
        else if (content.RootElement.ValueKind == JsonValueKind.Object && content.RootElement.TryGetProperty("text", out var textProp))
        {
            text = textProp.GetString();
        }

        if (string.IsNullOrWhiteSpace(text) || text.Length < 1 || text.Length > 500)
        {
            return CourseChatErrors.MessageLengthInvalid;
        }

        var message = new Message(
            Guid.NewGuid(),
            roomId,
            senderId,
            replyOnId,
            DateTimeOffset.UtcNow,
            content,
            []);

        return message;
    }

    public Result<Updated> AddReaction(React reaction)
    {
        var existing = _reacts.FirstOrDefault(r => r.ReactorId == reaction.ReactorId && r.Emoji == reaction.Emoji);
        if (existing == null)
        {
            _reacts.Add(reaction);
        }

        return Result.Updated;
    }

    public Result<Updated> RemoveReaction(Guid reactionId)
    {
        var existing = _reacts.FirstOrDefault(r => r.Id == reactionId);
        if (existing == null)
        {
            return CourseChatErrors.ReactionNotFound;
        }

        _reacts.Remove(existing);
        return Result.Updated;
    }
}
