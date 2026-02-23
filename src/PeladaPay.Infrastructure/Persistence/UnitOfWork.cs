using PeladaPay.Domain.Interfaces;
using PeladaPay.Infrastructure.Data;

namespace PeladaPay.Infrastructure.Persistence;

public class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
