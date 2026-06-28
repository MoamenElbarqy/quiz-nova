using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("CourseChatRoomMessages");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Content)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.HasOne(m => m.Room)
            .WithMany(r => r.Messages)
            .HasForeignKey(m => m.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(m => m.Reacts)
            .HasField("_reacts")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
