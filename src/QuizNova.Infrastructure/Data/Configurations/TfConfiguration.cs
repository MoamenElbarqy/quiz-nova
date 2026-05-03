using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using QuizNova.Domain.Entities.Quizzes.Questions.TrueFalse;

namespace QuizNova.Infrastructure.Data.Configurations;

public sealed class TfConfiguration : IEntityTypeConfiguration<Tf>
{
    public void Configure(EntityTypeBuilder<Tf> builder)
    {
        builder.ToTable("Tfs");
    }
}
