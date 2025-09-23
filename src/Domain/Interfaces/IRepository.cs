using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    System.Threading.Tasks.Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}