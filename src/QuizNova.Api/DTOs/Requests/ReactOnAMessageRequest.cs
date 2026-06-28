using System.Text.Json.Serialization;

namespace QuizNova.Api.DTOs.Requests;

public sealed record ReactOnAMessageRequest
{
    [JsonConstructor]
    public ReactOnAMessageRequest(Guid messageId, string emoji)
    {
        MessageId = messageId;
        Emoji = emoji;
    }

    public Guid MessageId { get; }

    public string Emoji { get; }

    public static ReactOnAMessageRequest Create(Guid messageId, string emoji)
    {
        return new ReactOnAMessageRequest(messageId, emoji);
    }
}
