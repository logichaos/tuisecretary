using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Interfaces;

namespace TuiSecretary.Infrastructure.Persistence;

public class InMemoryUnitOfWork : IUnitOfWork
{
    private readonly Lazy<IRepository<Note>> _notes = new(() => new InMemoryRepository<Note>());
    private readonly Lazy<IRepository<TodoList>> _todoLists = new(() => new InMemoryRepository<TodoList>());
    private readonly Lazy<IRepository<WorkTask>> _tasks = new(() => new InMemoryRepository<WorkTask>());
    private readonly Lazy<IRepository<CalendarEvent>> _calendarEvents = new(() => new InMemoryRepository<CalendarEvent>());

    public IRepository<Note> Notes => _notes.Value;
    public IRepository<TodoList> TodoLists => _todoLists.Value;
    public IRepository<WorkTask> Tasks => _tasks.Value;
    public IRepository<CalendarEvent> CalendarEvents => _calendarEvents.Value;

    public System.Threading.Tasks.Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // In-memory repository doesn't need explicit save
        return System.Threading.Tasks.Task.FromResult(0);
    }

    public System.Threading.Tasks.Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // No transaction support in memory
        return System.Threading.Tasks.Task.CompletedTask;
    }

    public System.Threading.Tasks.Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        // No transaction support in memory
        return System.Threading.Tasks.Task.CompletedTask;
    }

    public System.Threading.Tasks.Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        // No transaction support in memory
        return System.Threading.Tasks.Task.CompletedTask;
    }

    public void Dispose()
    {
        // Nothing to dispose for in-memory implementation
    }
}