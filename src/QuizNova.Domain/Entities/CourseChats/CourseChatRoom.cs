using QuizNova.Domain.Common;
using QuizNova.Domain.Common.Results;
using QuizNova.Domain.Entities.Users.Student;

namespace QuizNova.Domain.Entities.CourseChats;

public sealed class CourseChatRoom : Entity
{
    private readonly List<Student> _students;
    private readonly List<Message> _messages;

    private CourseChatRoom()
    {
    }

    private CourseChatRoom(
        Guid id,
        Guid courseId,
        Guid? instructorId,
        ChatStatus status,
        List<Student> students,
        List<Message> messages)
        : base(id)
    {
        CourseId = courseId;
        InstructorId = instructorId;
        Status = status;
        _students = students;
        _messages = messages;
    }

    public Guid CourseId { get; private set; }

    public Guid? InstructorId { get; private set; }

    public ChatStatus Status { get; private set; }

    public IEnumerable<Student> Students => _students.AsReadOnly();

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
            ChatStatus.OpenForAny,
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

    public Result<Updated> AddStudent(Student student)
    {
        if (_students.Any(s => s.Id == student.Id))
        {
            return CourseChatErrors.StudentAlreadyInRoom;
        }

        _students.Add(student);
        return Result.Updated;
    }

    public Result<Updated> RemoveStudent(Student student)
    {
        var existing = _students.FirstOrDefault(s => s.Id == student.Id);
        if (existing == null)
        {
            return CourseChatErrors.StudentNotInRoom;
        }

        _students.Remove(existing);
        return Result.Updated;
    }

    public Result<Updated> UpdateStatus(ChatStatus status)
    {
        Status = status;
        return Result.Updated;
    }

    public bool CanJoin(Guid userId)
    {
        return InstructorId == userId ||
               _students.Any(s => s.Id == userId);
    }

    public bool CanSend(Guid userId)
    {
        return Status switch
        {
            ChatStatus.OpenForAny => CanJoin(userId),
            ChatStatus.OpenForInstructor => InstructorId == userId,
            _ => false
        };
    }

    public bool CanReact(Guid userId)
    {
        return CanJoin(userId);
    }
}
