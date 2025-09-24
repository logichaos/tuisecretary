using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Interfaces;
using System.Collections.Concurrent;

namespace TuiSecretary.Infrastructure.Persistence;

public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ConcurrentDictionary<Guid, T> _entities = new();

    public System.Threading.Tasks.Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _entities.TryGetValue(id, out var entity);
        return System.Threading.Tasks.Task.FromResult(entity);
    }

    public System.Threading.Tasks.Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return System.Threading.Tasks.Task.FromResult(_entities.Values.AsEnumerable());
    }

    public System.Threading.Tasks.Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _entities.TryAdd(entity.Id, entity);
        return System.Threading.Tasks.Task.FromResult(entity);
    }

    public System.Threading.Tasks.Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _entities.TryUpdate(entity.Id, entity, _entities[entity.Id]);
        return System.Threading.Tasks.Task.FromResult(entity);
    }

    public System.Threading.Tasks.Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _entities.TryRemove(id, out _);
        return System.Threading.Tasks.Task.CompletedTask;
    }

    public System.Threading.Tasks.Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return System.Threading.Tasks.Task.FromResult(_entities.ContainsKey(id));
    }
}