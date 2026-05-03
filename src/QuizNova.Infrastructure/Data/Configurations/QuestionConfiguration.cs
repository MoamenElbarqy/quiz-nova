using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Quizzes.Questions.Base;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");
        builder.HasKey(q => q.Id);
        
        builder.Property(q => q.QuestionText)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(q => q.Marks)
            .IsRequired();
            
        builder.Property(q => q.DisplayOrder)
            .IsRequired();
    }
}
