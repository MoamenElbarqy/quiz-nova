using Microsoft.EntityFrameworkCore;

using QuizNova.Domain.Common;

namespace QuizNova.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<OutboxMessage> OutboxMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
