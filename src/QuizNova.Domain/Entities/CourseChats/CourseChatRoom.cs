using System.Text.Json;

using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;

namespace QuizNova.Domain.Entities.CourseChats;

public sealed class CourseChatRoom : Entity
{
    private List<Guid> _studentIds = [];
    private List<Message> _messages = [];

    private CourseChatRoom()
    {
    }

    private CourseChatRoom(
        Guid id,
        Guid courseId,
        Guid? instructorId,
        List<Guid> studentIds,
        List<Message> messages)
        : base(id)
    {
        CourseId = courseId;
        InstructorId = instructorId;
        _studentIds = studentIds;
        _messages = messages;
    }

    public Guid CourseId { get; private set; }

    public Guid? InstructorId { get; private set; }

    public IEnumerable<Guid> StudentIds => _studentIds.AsReadOnly();

    public IEnumerable<Message> Messages => _messages.AsReadOnly();

    public static Result<CourseChatRoom> Create(Guid courseId, Guid? instructorId)
    {
        if (courseId == Guid.Empty)
        {
            return CourseChatErrors.CourseIdRequired;
        }

        var chatRoom = new CourseChatRoom(
            Guid.NewGuid(),
            courseId,
            instructorId,
            [],
            []);

        return chatRoom;
    }

    public Result<Updated> UpdateInstructor(Guid? instructorId)
    {
        if (instructorId.HasValue && instructorId.Value == Guid.Empty)
        {
            return CourseChatErrors.InstructorIdRequired;
        }

        InstructorId = instructorId;
        return Result.Updated;
    }

    public Result<Updated> AddStudent(Guid studentId)
    {
        if (_studentIds.Contains(studentId))
        {
            return CourseChatErrors.StudentAlreadyInRoom;
        }

        _studentIds.Add(studentId);
        return Result.Updated;
    }

    public Result<Updated> RemoveStudent(Guid studentId)
    {
        if (!_studentIds.Remove(studentId))
        {
            return CourseChatErrors.StudentNotInRoom;
        }

        return Result.Updated;
    }

    public Result<Message> SendMessage(Guid senderId, Guid? replyOnId, JsonDocument content)
    {
        var messageResult = Message.Create(Id, senderId, replyOnId, content);
        if (messageResult.IsError)
        {
            return messageResult.Errors;
        }

        var message = messageResult.Value;
        _messages.Add(message);
        return message;
    }

    public bool CanJoin(Guid userId)
    {
        return InstructorId == userId ||
               _studentIds.Contains(userId);
    }

    public bool CanSend(Guid userId)
    {
        return CanJoin(userId);
    }

    public bool CanReact(Guid userId)
    {
        return CanJoin(userId);
    }
}
