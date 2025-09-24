using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Note> Notes { get; }
    IRepository<TodoList> TodoLists { get; }
    IRepository<WorkTask> Tasks { get; }
    IRepository<CalendarEvent> CalendarEvents { get; }
    
    System.Threading.Tasks.Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}