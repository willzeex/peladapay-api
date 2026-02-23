using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PeladaPay.Domain.Common;
using PeladaPay.Domain.Interfaces;
using PeladaPay.Infrastructure.Data;

namespace PeladaPay.Infrastructure.Persistence;

public class Repository<T>(ApplicationDbContext dbContext) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _dbSet = dbContext.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity) => _dbSet.Update(entity);
}
