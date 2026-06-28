using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuizNova.Api.DTOs.Requests;

public sealed record SendMessageRequest
{
    [JsonConstructor]
    public SendMessageRequest(Guid? replyOnId, JsonDocument content)
    {
        ReplyOnId = replyOnId;
        Content = content;
    }

    public Guid? ReplyOnId { get; }

    public JsonDocument Content { get; }

    public static SendMessageRequest Create(Guid? replyOnId, JsonDocument content)
    {
        return new SendMessageRequest(replyOnId, content);
    }
}
