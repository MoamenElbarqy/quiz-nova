using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.CourseChats;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class ReactConfiguration : IEntityTypeConfiguration<React>
{
    public void Configure(EntityTypeBuilder<React> builder)
    {
        builder.ToTable("MessageReactions");
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => new { r.MessageId, r.ReactorId })
            .IsUnique();

        builder.Property(r => r.Emoji)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(r => r.Message)
            .WithMany(m => m.Reacts)
            .HasForeignKey(r => r.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Reactor)
            .WithMany()
            .HasForeignKey(r => r.ReactorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
