using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.CourseChats;

public static class CourseChatErrors
{
    public static readonly Error CourseIdRequired =
        Error.Validation("CourseChatRoom.CourseIdRequired", "Course ID is required.");

    public static readonly Error RoomIdRequired =
        Error.Validation("Message.RoomIdRequired", "Room ID is required.");

    public static readonly Error SenderIdRequired =
        Error.Validation("Message.SenderIdRequired", "Sender ID is required.");

    public static readonly Error ContentRequired =
        Error.Validation("Message.ContentRequired", "Content is required.");

    public static readonly Error CannotJoin =
        Error.Forbidden("CourseChatRoom.CannotJoin", "You are not authorized to join this chat room.");

    public static readonly Error CannotSend =
        Error.Forbidden("CourseChatRoom.CannotSend", "You are not authorized to send messages in this chat room.");

    public static readonly Error CannotReact =
        Error.Forbidden("CourseChatRoom.CannotReact", "You are not authorized to react in this chat room.");

    public static readonly Error MessageIdRequired =
        Error.Validation("Reaction.MessageIdRequired", "Message ID is required.");

    public static readonly Error ReactorIdRequired =
        Error.Validation("Reaction.ReactorIdRequired", "Reactor ID is required.");

    public static readonly Error EmojiRequired =
        Error.Validation("Reaction.EmojiRequired", "Emoji is required.");

    public static readonly Error InstructorIdRequired =
        Error.Validation("CourseChatRoom.InstructorIdRequired", "Instructor ID is required.");

    public static readonly Error StudentAlreadyInRoom =
        Error.Conflict("CourseChatRoom.StudentAlreadyInRoom", "Student is already in this chat room.");

    public static readonly Error StudentNotInRoom =
        Error.NotFound("CourseChatRoom.StudentNotInRoom", "Student is not in this chat room.");

    public static readonly Error ReactionNotFound =
        Error.NotFound("Reaction.ReactionNotFound", "Reaction was not found.");

    public static readonly Error MessageLengthInvalid =
        Error.Validation("Message.LengthInvalid", "Message must be between 1 and 500 characters.");

    public static readonly Error EmojiInvalid =
        Error.Validation("Reaction.EmojiInvalid", "Emoji must be exactly one character.");
}
