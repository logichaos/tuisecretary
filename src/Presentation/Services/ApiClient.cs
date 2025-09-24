using System.Text;
using System.Text.Json;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Presentation.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IEnumerable<Note>> GetNotesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/notes", cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<IEnumerable<Note>>(json, _jsonOptions) ?? new List<Note>();
        }
        catch (Exception ex)
        {
            // Log error and return empty collection for graceful degradation
            Console.WriteLine($"Error fetching notes: {ex.Message}");
            return new List<Note>();
        }
    }

    public async Task<Note> CreateNoteAsync(string title, string content, List<string> tags, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new { Title = title, Content = content, Tags = tags };
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/notes", httpContent, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<Note>(responseJson, _jsonOptions) 
                ?? throw new InvalidOperationException("Failed to deserialize created note");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating note: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<WorkTask>> GetTasksAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/tasks", cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<IEnumerable<WorkTask>>(json, _jsonOptions) ?? new List<WorkTask>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching tasks: {ex.Message}");
            return new List<WorkTask>();
        }
    }

    public async Task<IEnumerable<CalendarEvent>> GetCalendarEventsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/calendar/events", cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<IEnumerable<CalendarEvent>>(json, _jsonOptions) ?? new List<CalendarEvent>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching calendar events: {ex.Message}");
            return new List<CalendarEvent>();
        }
    }

    public async Task<IEnumerable<TodoList>> GetTodoListsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/todolists", cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<IEnumerable<TodoList>>(json, _jsonOptions) ?? new List<TodoList>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching todo lists: {ex.Message}");
            return new List<TodoList>();
        }
    }
}