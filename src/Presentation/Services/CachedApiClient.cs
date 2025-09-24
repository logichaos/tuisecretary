using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Presentation.Services;

public class CachedApiClient : ICachedApiClient
{
    private readonly IApiClient _apiClient;
    private readonly Dictionary<string, (object Data, DateTime CachedAt)> _cache = new();
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5); // 5 minute cache
    private readonly object _lock = new object();

    public CachedApiClient(IApiClient apiClient)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    public async Task<IEnumerable<Note>> GetNotesAsync(CancellationToken cancellationToken = default)
    {
        return await GetCachedDataAsync("notes", 
            () => _apiClient.GetNotesAsync(cancellationToken),
            cancellationToken);
    }

    public async Task<Note> CreateNoteAsync(string title, string content, List<string> tags, CancellationToken cancellationToken = default)
    {
        var result = await _apiClient.CreateNoteAsync(title, content, tags, cancellationToken);
        
        // Invalidate notes cache after creating a new note
        lock (_lock)
        {
            _cache.Remove("notes");
        }
        
        return result;
    }

    public async Task<IEnumerable<WorkTask>> GetTasksAsync(CancellationToken cancellationToken = default)
    {
        return await GetCachedDataAsync("tasks", 
            () => _apiClient.GetTasksAsync(cancellationToken),
            cancellationToken);
    }

    public async Task<IEnumerable<CalendarEvent>> GetCalendarEventsAsync(CancellationToken cancellationToken = default)
    {
        return await GetCachedDataAsync("calendar", 
            () => _apiClient.GetCalendarEventsAsync(cancellationToken),
            cancellationToken);
    }

    public async Task<IEnumerable<TodoList>> GetTodoListsAsync(CancellationToken cancellationToken = default)
    {
        return await GetCachedDataAsync("todolists", 
            () => _apiClient.GetTodoListsAsync(cancellationToken),
            cancellationToken);
    }

    public void ClearCache()
    {
        lock (_lock)
        {
            _cache.Clear();
        }
    }

    public async Task RefreshAllAsync(CancellationToken cancellationToken = default)
    {
        ClearCache();
        
        // Pre-load all data
        await Task.WhenAll(
            GetNotesAsync(cancellationToken),
            GetTasksAsync(cancellationToken),
            GetCalendarEventsAsync(cancellationToken),
            GetTodoListsAsync(cancellationToken)
        );
    }

    private async Task<T> GetCachedDataAsync<T>(string cacheKey, Func<Task<T>> dataLoader, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(cacheKey, out var cached))
            {
                if (DateTime.Now - cached.CachedAt < _cacheExpiry)
                {
                    return (T)cached.Data;
                }
                else
                {
                    _cache.Remove(cacheKey);
                }
            }
        }

        // Load data from API
        var data = await dataLoader();
        
        lock (_lock)
        {
            _cache[cacheKey] = (data!, DateTime.Now);
        }
        
        return data;
    }
}