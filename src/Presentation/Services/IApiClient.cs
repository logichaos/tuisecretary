using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Presentation.Services;

public interface IApiClient
{
    // Notes endpoints
    Task<IEnumerable<Note>> GetNotesAsync(CancellationToken cancellationToken = default);
    Task<Note> CreateNoteAsync(string title, string content, List<string> tags, CancellationToken cancellationToken = default);

    // Tasks endpoints
    Task<IEnumerable<WorkTask>> GetTasksAsync(CancellationToken cancellationToken = default);

    // Calendar endpoints  
    Task<IEnumerable<CalendarEvent>> GetCalendarEventsAsync(CancellationToken cancellationToken = default);

    // TodoLists endpoints
    Task<IEnumerable<TodoList>> GetTodoListsAsync(CancellationToken cancellationToken = default);
}