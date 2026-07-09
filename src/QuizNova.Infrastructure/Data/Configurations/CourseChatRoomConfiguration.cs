using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class CourseChatRoomConfiguration : IEntityTypeConfiguration<CourseChatRoom>
{
    public void Configure(EntityTypeBuilder<CourseChatRoom> builder)
    {
        builder.ToTable("CourseChatRooms");
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.CourseId).IsUnique();

        builder.Navigation(c => c.Students)
            .HasField("_students")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(c => c.Messages)
            .HasField("_messages")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(c => c.Students)
            .WithMany()
            .UsingEntity("CourseChatRoomStudents");
    }
}
